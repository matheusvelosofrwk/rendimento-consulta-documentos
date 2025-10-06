using ConsultaDocumentos.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ConsultaDocumentos.Infra.Data.Services
{
    public class CertificadoManager : ICertificadoManager
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<CertificadoManager> _logger;
        private X509Certificate2? _certificadoCache;
        private readonly object _lock = new object();

        public bool CertificadoCarregado => _certificadoCache != null;
        public DateTime? DataExpiracao => _certificadoCache?.NotAfter;

        public CertificadoManager(
            IConfiguration configuration,
            ILogger<CertificadoManager> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public X509Certificate2 ObterCertificado()
        {
            if (_certificadoCache != null)
            {
                _logger.LogDebug("Retornando certificado do cache");
                return _certificadoCache;
            }

            lock (_lock)
            {
                if (_certificadoCache != null)
                {
                    return _certificadoCache;
                }

                _logger.LogInformation("Carregando certificado digital pela primeira vez");

                var caminhoArquivo = _configuration["ExternalProviders:Serpro:CertificadoPath"];
                var senhaCriptografada = _configuration["ExternalProviders:Serpro:CertificadoSenhaCriptografada"];
                var chaveCriptografia = _configuration["ExternalProviders:Serpro:ChaveCriptografia"];

                if (string.IsNullOrEmpty(caminhoArquivo))
                {
                    throw new InvalidOperationException(
                        "Caminho do certificado não configurado. Configure 'ExternalProviders:Serpro:CertificadoPath' no appsettings.json");
                }

                if (!File.Exists(caminhoArquivo))
                {
                    throw new FileNotFoundException(
                        $"Arquivo de certificado não encontrado: {caminhoArquivo}");
                }

                string senhaDescriptografada;

                if (!string.IsNullOrEmpty(senhaCriptografada) && !string.IsNullOrEmpty(chaveCriptografia))
                {
                    // Descriptografar senha
                    senhaDescriptografada = DescriptografarSenha(senhaCriptografada, chaveCriptografia);
                    _logger.LogDebug("Senha do certificado descriptografada com sucesso");
                }
                else
                {
                    // Senha em texto plano (desenvolvimento apenas)
                    senhaDescriptografada = _configuration["ExternalProviders:Serpro:CertificadoSenha"] ?? string.Empty;
                    _logger.LogWarning("Usando senha do certificado em texto plano - NÃO use em produção!");
                }

                try
                {
                    _certificadoCache = new X509Certificate2(
                        caminhoArquivo,
                        senhaDescriptografada,
                        X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);

                    _logger.LogInformation(
                        "Certificado carregado com sucesso. Válido até: {DataExpiracao}, Emissor: {Emissor}",
                        _certificadoCache.NotAfter,
                        _certificadoCache.Issuer);

                    // Verificar se o certificado está expirado
                    if (_certificadoCache.NotAfter < DateTime.Now)
                    {
                        _logger.LogError("ATENÇÃO: Certificado digital EXPIRADO em {DataExpiracao}", _certificadoCache.NotAfter);
                        throw new InvalidOperationException($"Certificado digital expirado em {_certificadoCache.NotAfter}");
                    }

                    // Alertar se estiver próximo do vencimento (30 dias)
                    var diasParaVencimento = (_certificadoCache.NotAfter - DateTime.Now).Days;
                    if (diasParaVencimento < 30)
                    {
                        _logger.LogWarning(
                            "ATENÇÃO: Certificado digital vence em {Dias} dias ({DataExpiracao})",
                            diasParaVencimento,
                            _certificadoCache.NotAfter);
                    }

                    return _certificadoCache;
                }
                catch (CryptographicException ex)
                {
                    _logger.LogError(ex, "Erro ao carregar certificado. Verifique o arquivo e a senha.");
                    throw new InvalidOperationException(
                        "Falha ao carregar certificado digital. Verifique o arquivo e a senha.", ex);
                }
            }
        }

        public Task<X509Certificate2> ObterCertificadoAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(ObterCertificado());
        }

        private string DescriptografarSenha(string senhaCriptografada, string chave)
        {
            try
            {
                using (Aes aes = Aes.Create())
                {
                    // Usar a chave fornecida (deve ter 32 bytes para AES-256)
                    var keyBytes = Encoding.UTF8.GetBytes(chave.PadRight(32).Substring(0, 32));
                    aes.Key = keyBytes;

                    // IV padrão (em produção, deve ser armazenado junto com os dados criptografados)
                    aes.IV = new byte[16]; // IV de zeros (simplificado - melhorar em produção)

                    using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                    {
                        var encryptedBytes = Convert.FromBase64String(senhaCriptografada);
                        var decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                        return Encoding.UTF8.GetString(decryptedBytes);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao descriptografar senha do certificado");
                throw new InvalidOperationException("Falha ao descriptografar senha do certificado", ex);
            }
        }
    }
}

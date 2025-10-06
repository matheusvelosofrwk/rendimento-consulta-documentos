using ConsultaDocumentos.Application.DTOs.External.Serasa;
using ConsultaDocumentos.Application.Exceptions;
using ConsultaDocumentos.Application.Helpers;
using ConsultaDocumentos.Application.Interfaces;
using ConsultaDocumentos.Domain.Intefaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text;

namespace ConsultaDocumentos.Application.Services.External
{
    public class SerasaService : ISerasaService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IProvedorRepository _provedorRepository;
        private readonly ILogger<SerasaService> _logger;

        private const string SERASA_NOME = "SERASA";
        private const string NAMESPACE_URI = "http://www.experianmarketing.com.br/";

        public SerasaService(
            HttpClient httpClient,
            IConfiguration configuration,
            IProvedorRepository provedorRepository,
            ILogger<SerasaService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _provedorRepository = provedorRepository;
            _logger = logger;
        }

        public async Task<SerasaCPFResponse> ConsultarCPFAsync(
            string cpf,
            int idSistema,
            CancellationToken cancellationToken = default)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                _logger.LogInformation("Iniciando consulta CPF no Serasa: {CPF}", cpf);

                // Obter credenciais do provedor no banco de dados
                var provedor = await ObterProvedorSerasaAsync();

                // Montar requisição SOAP
                var parametros = new Dictionary<string, string>
                {
                    { "strLogin", provedor.Usuario ?? string.Empty },
                    { "strSenha", provedor.Senha ?? string.Empty },
                    { "strDominio", provedor.Dominio ?? string.Empty },
                    { "strDocumento", cpf }
                };

                var soapEnvelope = SoapHelper.CriarEnvelopeSerasa("retornaDadosPF", parametros, NAMESPACE_URI);

                // Enviar requisição
                var endpointUrl = _configuration["ExternalProviders:Serasa:Real:EndpointUrl"];
                if (string.IsNullOrEmpty(endpointUrl))
                {
                    throw new InvalidOperationException("Endpoint Serasa não configurado");
                }

                var content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml");
                content.Headers.Add("SOAPAction", $"{NAMESPACE_URI}retornaDadosPF");

                _logger.LogDebug("Enviando requisição SOAP para {Url}", endpointUrl);

                var httpResponse = await _httpClient.PostAsync(endpointUrl, content, cancellationToken);
                httpResponse.EnsureSuccessStatusCode();

                var responseXml = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

                _logger.LogDebug("Resposta SOAP recebida em {Tempo}ms", stopwatch.ElapsedMilliseconds);

                // Parsear resposta XML
                var doc = SoapHelper.ParsearRespostaSoap(responseXml);

                // Extrair dados
                var response = new SerasaCPFResponse
                {
                    CPF = SoapHelper.ExtrairValorXml(doc, "CPF"),
                    Nome = SoapHelper.ExtrairValorXml(doc, "Nome"),
                    Situacao = SoapHelper.ExtrairValorXml(doc, "Situacao"),
                    Codigo_de_Controle = SoapHelper.ExtrairValorXml(doc, "Codigo_de_Controle"),
                    Hora = SoapHelper.ExtrairValorXml(doc, "Hora"),
                    Fonte_Pesquisada = SoapHelper.ExtrairValorXml(doc, "Fonte_Pesquisada"),
                    Erro = SoapHelper.ExtrairErroSoap(doc)
                };

                // Validações conforme documentação
                if (response.TemErro)
                {
                    _logger.LogWarning("Erro retornado pelo Serasa: {Erro}", response.Erro);
                    throw new ExternalProviderException(SERASA_NOME, response.Erro ?? "Erro desconhecido");
                }

                if (string.IsNullOrWhiteSpace(response.Nome))
                {
                    _logger.LogWarning("Nome em branco retornado pelo Serasa para CPF {CPF}", cpf);
                    throw new ExternalProviderException(SERASA_NOME, "Nome em branco");
                }

                _logger.LogInformation("Consulta CPF Serasa concluída com sucesso em {Tempo}ms", stopwatch.ElapsedMilliseconds);

                return response;
            }
            catch (ExternalProviderException)
            {
                throw;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Erro de comunicação com Serasa");
                throw new ExternalProviderException(SERASA_NOME, "Falha na comunicação com o serviço", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao consultar CPF no Serasa");
                throw new ExternalProviderException(SERASA_NOME, "Erro inesperado", ex);
            }
        }

        public async Task<SerasaCNPJResponse> ConsultarCNPJAsync(
            string cnpj,
            int idSistema,
            CancellationToken cancellationToken = default)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                _logger.LogInformation("Iniciando consulta CNPJ no Serasa: {CNPJ}", cnpj);

                // Obter credenciais do provedor no banco de dados
                var provedor = await ObterProvedorSerasaAsync();

                // Montar requisição SOAP
                var parametros = new Dictionary<string, string>
                {
                    { "strLogin", provedor.Usuario ?? string.Empty },
                    { "strSenha", provedor.Senha ?? string.Empty },
                    { "strDominio", provedor.Dominio ?? string.Empty },
                    { "strDocumento", cnpj }
                };

                var soapEnvelope = SoapHelper.CriarEnvelopeSerasa("retornaDadosPJ", parametros, NAMESPACE_URI);

                // Enviar requisição
                var endpointUrl = _configuration["ExternalProviders:Serasa:Real:EndpointUrl"];
                if (string.IsNullOrEmpty(endpointUrl))
                {
                    throw new InvalidOperationException("Endpoint Serasa não configurado");
                }

                var content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml");
                content.Headers.Add("SOAPAction", $"{NAMESPACE_URI}retornaDadosPJ");

                _logger.LogDebug("Enviando requisição SOAP para {Url}", endpointUrl);

                var httpResponse = await _httpClient.PostAsync(endpointUrl, content, cancellationToken);
                httpResponse.EnsureSuccessStatusCode();

                var responseXml = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

                _logger.LogDebug("Resposta SOAP recebida em {Tempo}ms", stopwatch.ElapsedMilliseconds);

                // Parsear resposta XML
                var doc = SoapHelper.ParsearRespostaSoap(responseXml);

                // Extrair dados
                var response = new SerasaCNPJResponse
                {
                    CNPJ = SoapHelper.ExtrairValorXml(doc, "CNPJ"),
                    Nome = SoapHelper.ExtrairValorXml(doc, "Nome"),
                    DataAbertura = SoapHelper.ExtrairValorXml(doc, "DataAbertura"),
                    Situacao = SoapHelper.ExtrairValorXml(doc, "Situacao"),
                    DataSituacao = SoapHelper.ExtrairValorXml(doc, "DataSituacao"),
                    Data = SoapHelper.ExtrairValorXml(doc, "Data"),
                    Hora = SoapHelper.ExtrairValorXml(doc, "Hora"),
                    Erro = SoapHelper.ExtrairErroSoap(doc)
                };

                // Validações conforme documentação
                if (response.TemErro)
                {
                    _logger.LogWarning("Erro retornado pelo Serasa: {Erro}", response.Erro);
                    throw new ExternalProviderException(SERASA_NOME, response.Erro ?? "Erro desconhecido");
                }

                if (string.IsNullOrWhiteSpace(response.Nome))
                {
                    _logger.LogWarning("Nome em branco retornado pelo Serasa para CNPJ {CNPJ}", cnpj);
                    throw new ExternalProviderException(SERASA_NOME, "Nome em branco");
                }

                _logger.LogInformation("Consulta CNPJ Serasa concluída com sucesso em {Tempo}ms", stopwatch.ElapsedMilliseconds);

                return response;
            }
            catch (ExternalProviderException)
            {
                throw;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Erro de comunicação com Serasa");
                throw new ExternalProviderException(SERASA_NOME, "Falha na comunicação com o serviço", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao consultar CNPJ no Serasa");
                throw new ExternalProviderException(SERASA_NOME, "Erro inesperado", ex);
            }
        }

        public async Task<SerasaHealthCheckResponse> VerificarDisponibilidadeAsync(
            CancellationToken cancellationToken = default)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = new SerasaHealthCheckResponse
            {
                Timestamp = DateTime.UtcNow
            };

            try
            {
                // Verificar se o endpoint está acessível
                var endpointUrl = _configuration["ExternalProviders:Serasa:Real:EndpointUrl"];
                if (string.IsNullOrEmpty(endpointUrl))
                {
                    response.Status = "ERROR";
                    response.Mensagem = "Endpoint não configurado";
                    return response;
                }

                // Fazer uma requisição HEAD ou GET simples
                var httpResponse = await _httpClient.GetAsync($"{endpointUrl}?wsdl", cancellationToken);

                stopwatch.Stop();

                if (httpResponse.IsSuccessStatusCode)
                {
                    response.Status = "OK";
                    response.Mensagem = "Serviço disponível";
                    response.TempoRespostaMs = stopwatch.ElapsedMilliseconds;
                }
                else
                {
                    response.Status = "ERROR";
                    response.Mensagem = $"HTTP {(int)httpResponse.StatusCode}";
                    response.TempoRespostaMs = stopwatch.ElapsedMilliseconds;
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                response.Status = "ERROR";
                response.Mensagem = ex.Message;
                response.TempoRespostaMs = stopwatch.ElapsedMilliseconds;
                _logger.LogWarning(ex, "Serasa indisponível");
            }

            return response;
        }

        private async Task<Domain.Entities.Provedor> ObterProvedorSerasaAsync()
        {
            var provedor = await _provedorRepository.GetByNomeAsync(SERASA_NOME);
            if (provedor == null)
            {
                throw new InvalidOperationException($"Provedor {SERASA_NOME} não encontrado no banco de dados");
            }

            if (string.IsNullOrEmpty(provedor.Usuario) || string.IsNullOrEmpty(provedor.Senha) || string.IsNullOrEmpty(provedor.Dominio))
            {
                throw new InvalidOperationException($"Credenciais do provedor {SERASA_NOME} não configuradas");
            }

            return provedor;
        }
    }
}

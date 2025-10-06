using ConsultaDocumentos.Application.DTOs.External.Serasa;
using ConsultaDocumentos.Application.Exceptions;
using ConsultaDocumentos.Application.Helpers;
using ConsultaDocumentos.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text;
using System.Text.Json;

namespace ConsultaDocumentos.Application.Services.External
{
    public class SerasaService : ISerasaService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SerasaService> _logger;
        private readonly string _baseUrl;
        private readonly string _usuario;
        private readonly string _senha;

        public SerasaService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<SerasaService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;

            _baseUrl = _configuration["ExternalProviders:Serasa:BaseUrl"]
                ?? throw new InvalidOperationException("SERASA BaseUrl não configurada");
            _usuario = _configuration["ExternalProviders:Serasa:Usuario"]
                ?? throw new InvalidOperationException("SERASA Usuario não configurado");
            _senha = _configuration["ExternalProviders:Serasa:Senha"]
                ?? throw new InvalidOperationException("SERASA Senha não configurada");

            ConfigureHttpClient();
        }

        private void ConfigureHttpClient()
        {
            _httpClient.BaseAddress = new Uri(_baseUrl);
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "ConsultaDocumentos/1.0");
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public async Task<SerasaCPFResponse> ConsultarCPFAsync(
            string cpf,
            string tipoConsulta = "COMPLETA",
            CancellationToken cancellationToken = default)
        {
            try
            {
                var cpfLimpo = DocumentoValidationHelper.RemoverFormatacao(cpf);
                if (!DocumentoValidationHelper.ValidarCPF(cpfLimpo))
                {
                    throw new ExternalProviderException("SERASA", "CPF inválido");
                }

                var request = new SerasaCPFRequest
                {
                    Cpf = cpfLimpo,
                    TipoConsulta = tipoConsulta,
                    Usuario = _usuario,
                    Senha = _senha
                };

                _logger.LogInformation("Consultando CPF {CPF} na SERASA (Tipo: {Tipo})",
                    MascararCPF(cpfLimpo), tipoConsulta);

                var response = await PostAsync<SerasaCPFRequest, SerasaCPFResponse>(
                    "/cpf/consultar",
                    request,
                    cancellationToken);

                if (response.Status != "SUCESSO")
                {
                    _logger.LogWarning("SERASA retornou erro para CPF {CPF}: {Mensagem}",
                        MascararCPF(cpfLimpo), response.Mensagem);
                    throw new ExternalProviderException("SERASA", response.Mensagem);
                }

                _logger.LogInformation("CPF consultado com sucesso na SERASA");
                return response;
            }
            catch (ExternalProviderException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar CPF na SERASA");
                throw new ExternalProviderException("SERASA", "Erro ao consultar CPF", ex);
            }
        }

        public async Task<SerasaCNPJResponse> ConsultarCNPJAsync(
            string cnpj,
            string tipoConsulta = "COMPLETA",
            CancellationToken cancellationToken = default)
        {
            try
            {
                var cnpjLimpo = DocumentoValidationHelper.RemoverFormatacao(cnpj);
                if (!DocumentoValidationHelper.ValidarCNPJ(cnpjLimpo))
                {
                    throw new ExternalProviderException("SERASA", "CNPJ inválido");
                }

                var request = new SerasaCNPJRequest
                {
                    Cnpj = cnpjLimpo,
                    TipoConsulta = tipoConsulta,
                    Usuario = _usuario,
                    Senha = _senha
                };

                _logger.LogInformation("Consultando CNPJ {CNPJ} na SERASA (Tipo: {Tipo})",
                    MascararCNPJ(cnpjLimpo), tipoConsulta);

                var response = await PostAsync<SerasaCNPJRequest, SerasaCNPJResponse>(
                    "/cnpj/consultar",
                    request,
                    cancellationToken);

                if (response.Status != "SUCESSO")
                {
                    _logger.LogWarning("SERASA retornou erro para CNPJ {CNPJ}: {Mensagem}",
                        MascararCNPJ(cnpjLimpo), response.Mensagem);
                    throw new ExternalProviderException("SERASA", response.Mensagem);
                }

                _logger.LogInformation("CNPJ consultado com sucesso na SERASA");
                return response;
            }
            catch (ExternalProviderException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar CNPJ na SERASA");
                throw new ExternalProviderException("SERASA", "Erro ao consultar CNPJ", ex);
            }
        }

        public async Task<SerasaScoreResponse> ConsultarScoreAsync(
            string documento,
            string tipoDocumento,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var documentoLimpo = DocumentoValidationHelper.RemoverFormatacao(documento);

                // Valida documento conforme o tipo
                if (tipoDocumento.ToUpperInvariant() == "CPF")
                {
                    if (!DocumentoValidationHelper.ValidarCPF(documentoLimpo))
                    {
                        throw new ExternalProviderException("SERASA", "CPF inválido");
                    }
                }
                else if (tipoDocumento.ToUpperInvariant() == "CNPJ")
                {
                    if (!DocumentoValidationHelper.ValidarCNPJ(documentoLimpo))
                    {
                        throw new ExternalProviderException("SERASA", "CNPJ inválido");
                    }
                }
                else
                {
                    throw new ExternalProviderException("SERASA", "Tipo de documento inválido. Use CPF ou CNPJ");
                }

                var request = new SerasaScoreRequest
                {
                    Documento = documentoLimpo,
                    TipoDocumento = tipoDocumento.ToUpperInvariant(),
                    Usuario = _usuario,
                    Senha = _senha
                };

                _logger.LogInformation("Consultando Score de {Tipo} {Documento} na SERASA",
                    tipoDocumento, MascararDocumento(documentoLimpo, tipoDocumento));

                var response = await PostAsync<SerasaScoreRequest, SerasaScoreResponse>(
                    "/score/consultar",
                    request,
                    cancellationToken);

                if (response.Status != "SUCESSO")
                {
                    _logger.LogWarning("SERASA retornou erro ao consultar score: {Mensagem}",
                        response.Mensagem);
                    throw new ExternalProviderException("SERASA", response.Mensagem);
                }

                _logger.LogInformation("Score consultado com sucesso na SERASA");
                return response;
            }
            catch (ExternalProviderException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar score na SERASA");
                throw new ExternalProviderException("SERASA", "Erro ao consultar score", ex);
            }
        }

        public async Task<SerasaHealthCheckResponse> HealthCheckAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Verificando saúde da SERASA");

                var response = await _httpClient.GetAsync("/health", cancellationToken);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                var healthCheck = JsonSerializer.Deserialize<SerasaHealthCheckResponse>(
                    content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                _logger.LogInformation("SERASA está saudável: {Status}", healthCheck?.Status);
                return healthCheck ?? throw new ExternalProviderException("SERASA", "Resposta de health check inválida");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "SERASA indisponível no health check");
                throw new ExternalProviderException("SERASA", "Serviço indisponível", ex,
                    ex.StatusCode ?? HttpStatusCode.ServiceUnavailable);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar saúde da SERASA");
                throw new ExternalProviderException("SERASA", "Erro no health check", ex);
            }
        }

        private async Task<TResponse> PostAsync<TRequest, TResponse>(
            string endpoint,
            TRequest request,
            CancellationToken cancellationToken)
        {
            var jsonContent = JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new ExternalProviderException(
                    "SERASA",
                    $"Erro HTTP {response.StatusCode}",
                    response.StatusCode,
                    responseContent);
            }

            var result = JsonSerializer.Deserialize<TResponse>(responseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result ?? throw new ExternalProviderException("SERASA", "Resposta vazia ou inválida");
        }

        private string MascararCPF(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf) || cpf.Length != 11)
                return cpf;

            return $"{cpf.Substring(0, 3)}.***.**{cpf.Substring(9, 2)}";
        }

        private string MascararCNPJ(string cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj) || cnpj.Length != 14)
                return cnpj;

            return $"{cnpj.Substring(0, 2)}.***.***/****-{cnpj.Substring(12, 2)}";
        }

        private string MascararDocumento(string documento, string tipo)
        {
            return tipo.ToUpperInvariant() == "CPF"
                ? MascararCPF(documento)
                : MascararCNPJ(documento);
        }
    }
}

using ConsultaDocumentos.Application.DTOs.External.Serpro;
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
    public class SerproService : ISerproService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SerproService> _logger;
        private readonly string _baseUrl;
        private readonly string _cpfOperador;

        public SerproService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<SerproService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;

            _baseUrl = _configuration["ExternalProviders:Serpro:BaseUrl"]
                ?? throw new InvalidOperationException("SERPRO BaseUrl não configurada");
            _cpfOperador = _configuration["ExternalProviders:Serpro:CpfOperador"]
                ?? throw new InvalidOperationException("SERPRO CpfOperador não configurado");

            ConfigureHttpClient();
        }

        private void ConfigureHttpClient()
        {
            _httpClient.BaseAddress = new Uri(_baseUrl);
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "ConsultaDocumentos/1.0");
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public async Task<SerprocCPFResponse> ConsultarCPFAsync(
            string cpf,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Valida CPF
                var cpfLimpo = DocumentoValidationHelper.RemoverFormatacao(cpf);
                if (!DocumentoValidationHelper.ValidarCPF(cpfLimpo))
                {
                    throw new ExternalProviderException("SERPRO", "CPF inválido");
                }

                var request = new SerprocCPFRequest
                {
                    ListaDeCpf = cpfLimpo,
                    CpfUsuario = _cpfOperador
                };

                _logger.LogInformation("Consultando CPF {CPF} no SERPRO", MascararCPF(cpfLimpo));

                var response = await PostAsync<SerprocCPFRequest, SerprocCPFResponse>(
                    "/cpf/ConsultarCPFP1",
                    request,
                    cancellationToken);

                if (!string.IsNullOrWhiteSpace(response.Erro))
                {
                    _logger.LogWarning("SERPRO retornou erro para CPF {CPF}: {Erro}",
                        MascararCPF(cpfLimpo), response.Erro);
                    throw new ExternalProviderException("SERPRO", response.Erro);
                }

                _logger.LogInformation("CPF consultado com sucesso no SERPRO");
                return response;
            }
            catch (ExternalProviderException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar CPF no SERPRO");
                throw new ExternalProviderException("SERPRO", "Erro ao consultar CPF", ex);
            }
        }

        public async Task<SerprocCNPJPerfil1Response> ConsultarCNPJPerfil1Async(
            string cnpj,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var cnpjLimpo = DocumentoValidationHelper.RemoverFormatacao(cnpj);
                if (!DocumentoValidationHelper.ValidarCNPJ(cnpjLimpo))
                {
                    throw new ExternalProviderException("SERPRO", "CNPJ inválido");
                }

                var request = new SerprocCNPJRequest
                {
                    ListaDeCnpj = cnpjLimpo,
                    CpfUsuario = _cpfOperador
                };

                _logger.LogInformation("Consultando CNPJ {CNPJ} Perfil 1 no SERPRO", MascararCNPJ(cnpjLimpo));

                var response = await PostAsync<SerprocCNPJRequest, SerprocCNPJPerfil1Response>(
                    "/cnpj/ConsultarCNPJP1",
                    request,
                    cancellationToken);

                if (!string.IsNullOrWhiteSpace(response.Erro))
                {
                    _logger.LogWarning("SERPRO retornou erro para CNPJ {CNPJ}: {Erro}",
                        MascararCNPJ(cnpjLimpo), response.Erro);
                    throw new ExternalProviderException("SERPRO", response.Erro);
                }

                _logger.LogInformation("CNPJ Perfil 1 consultado com sucesso no SERPRO");
                return response;
            }
            catch (ExternalProviderException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar CNPJ Perfil 1 no SERPRO");
                throw new ExternalProviderException("SERPRO", "Erro ao consultar CNPJ Perfil 1", ex);
            }
        }

        public async Task<SerprocCNPJPerfil2Response> ConsultarCNPJPerfil2Async(
            string cnpj,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var cnpjLimpo = DocumentoValidationHelper.RemoverFormatacao(cnpj);
                if (!DocumentoValidationHelper.ValidarCNPJ(cnpjLimpo))
                {
                    throw new ExternalProviderException("SERPRO", "CNPJ inválido");
                }

                var request = new SerprocCNPJRequest
                {
                    ListaDeCnpj = cnpjLimpo,
                    CpfUsuario = _cpfOperador
                };

                _logger.LogInformation("Consultando CNPJ {CNPJ} Perfil 2 no SERPRO", MascararCNPJ(cnpjLimpo));

                var response = await PostAsync<SerprocCNPJRequest, SerprocCNPJPerfil2Response>(
                    "/cnpj/ConsultarCNPJP2",
                    request,
                    cancellationToken);

                if (!string.IsNullOrWhiteSpace(response.Erro))
                {
                    _logger.LogWarning("SERPRO retornou erro para CNPJ {CNPJ}: {Erro}",
                        MascararCNPJ(cnpjLimpo), response.Erro);
                    throw new ExternalProviderException("SERPRO", response.Erro);
                }

                _logger.LogInformation("CNPJ Perfil 2 consultado com sucesso no SERPRO");
                return response;
            }
            catch (ExternalProviderException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar CNPJ Perfil 2 no SERPRO");
                throw new ExternalProviderException("SERPRO", "Erro ao consultar CNPJ Perfil 2", ex);
            }
        }

        public async Task<SerprocCNPJPerfil3Response> ConsultarCNPJPerfil3Async(
            string cnpj,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var cnpjLimpo = DocumentoValidationHelper.RemoverFormatacao(cnpj);
                if (!DocumentoValidationHelper.ValidarCNPJ(cnpjLimpo))
                {
                    throw new ExternalProviderException("SERPRO", "CNPJ inválido");
                }

                var request = new SerprocCNPJRequest
                {
                    ListaDeCnpj = cnpjLimpo,
                    CpfUsuario = _cpfOperador
                };

                _logger.LogInformation("Consultando CNPJ {CNPJ} Perfil 3 no SERPRO", MascararCNPJ(cnpjLimpo));

                var response = await PostAsync<SerprocCNPJRequest, SerprocCNPJPerfil3Response>(
                    "/cnpj/ConsultarCNPJP3",
                    request,
                    cancellationToken);

                if (!string.IsNullOrWhiteSpace(response.Erro))
                {
                    _logger.LogWarning("SERPRO retornou erro para CNPJ {CNPJ}: {Erro}",
                        MascararCNPJ(cnpjLimpo), response.Erro);
                    throw new ExternalProviderException("SERPRO", response.Erro);
                }

                _logger.LogInformation("CNPJ Perfil 3 consultado com sucesso no SERPRO");
                return response;
            }
            catch (ExternalProviderException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar CNPJ Perfil 3 no SERPRO");
                throw new ExternalProviderException("SERPRO", "Erro ao consultar CNPJ Perfil 3", ex);
            }
        }

        public async Task<SerproHealthCheckResponse> HealthCheckAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Verificando saúde do SERPRO");

                var response = await _httpClient.GetAsync("/health", cancellationToken);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                var healthCheck = JsonSerializer.Deserialize<SerproHealthCheckResponse>(
                    content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                _logger.LogInformation("SERPRO está saudável: {Status}", healthCheck?.Status);
                return healthCheck ?? throw new ExternalProviderException("SERPRO", "Resposta de health check inválida");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "SERPRO indisponível no health check");
                throw new ExternalProviderException("SERPRO", "Serviço indisponível", ex,
                    ex.StatusCode ?? HttpStatusCode.ServiceUnavailable);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar saúde do SERPRO");
                throw new ExternalProviderException("SERPRO", "Erro no health check", ex);
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
                    "SERPRO",
                    $"Erro HTTP {response.StatusCode}",
                    response.StatusCode,
                    responseContent);
            }

            var result = JsonSerializer.Deserialize<TResponse>(responseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result ?? throw new ExternalProviderException("SERPRO", "Resposta vazia ou inválida");
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
    }
}

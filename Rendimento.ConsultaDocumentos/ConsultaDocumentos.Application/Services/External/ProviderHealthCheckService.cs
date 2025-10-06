using ConsultaDocumentos.Application.DTOs.External;
using ConsultaDocumentos.Application.Exceptions;
using ConsultaDocumentos.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace ConsultaDocumentos.Application.Services.External
{
    public class ProviderHealthCheckService : IProviderHealthCheckService
    {
        private readonly ISerproServiceMock _serproService;
        private readonly ISerasaServiceMock _serasaService;
        private readonly ILogger<ProviderHealthCheckService> _logger;

        public ProviderHealthCheckService(
            ISerproServiceMock serproService,
            ISerasaServiceMock serasaService,
            ILogger<ProviderHealthCheckService> logger)
        {
            _serproService = serproService;
            _serasaService = serasaService;
            _logger = logger;
        }

        public async Task<ProvidersHealthCheckResponse> CheckAllProvidersAsync(
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Iniciando verificação de saúde de todos os provedores");

            var checkTasks = new[]
            {
                CheckProviderAsync("SERPRO", cancellationToken),
                CheckProviderAsync("SERASA", cancellationToken)
            };

            var results = await Task.WhenAll(checkTasks);

            var response = new ProvidersHealthCheckResponse
            {
                TotalProvedores = results.Length,
                ProvedoresDisponiveis = results.Count(r => r.Disponivel),
                Provedores = results.ToList(),
                DataVerificacao = DateTime.UtcNow,
                AlgumProvedorDisponivel = results.Any(r => r.Disponivel)
            };

            _logger.LogInformation(
                "Verificação concluída: {Disponiveis}/{Total} provedores disponíveis",
                response.ProvedoresDisponiveis,
                response.TotalProvedores);

            return response;
        }

        public async Task<ProviderHealthStatusDTO> CheckProviderAsync(
            string providerName,
            CancellationToken cancellationToken = default)
        {
            var stopwatch = Stopwatch.StartNew();
            var status = new ProviderHealthStatusDTO
            {
                NomeProvedor = providerName,
                DataVerificacao = DateTime.UtcNow
            };

            try
            {
                _logger.LogInformation("Verificando saúde do provedor {Provider}", providerName);

                switch (providerName.ToUpperInvariant())
                {
                    case "SERPRO":
                        var serproHealth = await _serproService.HealthCheckAsync(cancellationToken);
                        status.Status = serproHealth.Status;
                        status.Disponivel = serproHealth.Status.ToLowerInvariant() == "OK".ToLowerInvariant();
                        status.Mensagem = $"Verificado em {serproHealth.Timestamp}";
                        break;

                    case "SERASA":
                        var serasaHealth = await _serasaService.HealthCheckAsync(cancellationToken);
                        status.Status = serasaHealth.Status;
                        status.Disponivel = serasaHealth.Status.ToLowerInvariant() == "OK".ToLowerInvariant();
                        status.Mensagem = $"Verificado em {serasaHealth.Timestamp}";
                        break;

                    default:
                        status.Status = "unknown";
                        status.Disponivel = false;
                        status.Mensagem = $"Provedor '{providerName}' não reconhecido";
                        _logger.LogWarning("Provedor desconhecido: {Provider}", providerName);
                        break;
                }

                stopwatch.Stop();
                status.TempoRespostaMs = stopwatch.ElapsedMilliseconds;

                _logger.LogInformation(
                    "Provedor {Provider} - Status: {Status}, Disponível: {Disponivel}, Tempo: {Tempo}ms",
                    providerName, status.Status, status.Disponivel, status.TempoRespostaMs);
            }
            catch (ExternalProviderException ex)
            {
                stopwatch.Stop();
                status.Status = "error";
                status.Disponivel = false;
                status.Mensagem = ex.Message;
                status.TempoRespostaMs = stopwatch.ElapsedMilliseconds;

                _logger.LogWarning(ex,
                    "Provedor {Provider} indisponível: {Message}",
                    providerName, ex.Message);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                status.Status = "error";
                status.Disponivel = false;
                status.Mensagem = $"Erro ao verificar provedor: {ex.Message}";
                status.TempoRespostaMs = stopwatch.ElapsedMilliseconds;

                _logger.LogError(ex,
                    "Erro inesperado ao verificar provedor {Provider}",
                    providerName);
            }

            return status;
        }
    }
}

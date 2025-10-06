using ConsultaDocumentos.Application.DTOs.External;
using ConsultaDocumentos.Application.Exceptions;
using ConsultaDocumentos.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace ConsultaDocumentos.Application.Services.External
{
    public class ProviderHealthCheckService : IProviderHealthCheckService
    {
        private readonly ISerproServiceMock _serproServiceMock;
        private readonly ISerasaServiceMock _serasaServiceMock;
        private readonly ISerproService? _serproServiceReal;
        private readonly ISerasaService? _serasaServiceReal;
        private readonly IProviderSelector _providerSelector;
        private readonly ILogger<ProviderHealthCheckService> _logger;

        public ProviderHealthCheckService(
            ISerproServiceMock serproServiceMock,
            ISerasaServiceMock serasaServiceMock,
            IProviderSelector providerSelector,
            ILogger<ProviderHealthCheckService> logger,
            ISerproService? serproServiceReal = null,
            ISerasaService? serasaServiceReal = null)
        {
            _serproServiceMock = serproServiceMock;
            _serasaServiceMock = serasaServiceMock;
            _serproServiceReal = serproServiceReal;
            _serasaServiceReal = serasaServiceReal;
            _providerSelector = providerSelector;
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
                _logger.LogInformation("Verificando saúde do provedor {Provider} (modo: {Modo})",
                    providerName, _providerSelector.ObterModoAtual());

                var usarMock = _providerSelector.UsarMock();

                switch (providerName.ToUpperInvariant())
                {
                    case "SERPRO":
                        if (usarMock)
                        {
                            var serproHealth = await _serproServiceMock.HealthCheckAsync(cancellationToken);
                            status.Status = serproHealth.Status;
                            status.Disponivel = serproHealth.Status.ToLowerInvariant() == "OK".ToLowerInvariant();
                            status.Mensagem = $"MOCK - Verificado em {serproHealth.Timestamp}";
                        }
                        else
                        {
                            if (_serproServiceReal == null)
                            {
                                status.Status = "ERROR";
                                status.Disponivel = false;
                                status.Mensagem = "Serviço real SERPRO não configurado";
                            }
                            else
                            {
                                var serproHealth = await _serproServiceReal.VerificarDisponibilidadeAsync(cancellationToken);
                                status.Status = serproHealth.Status;
                                status.Disponivel = serproHealth.Disponivel;
                                status.Mensagem = $"REAL - {serproHealth.Mensagem}";
                            }
                        }
                        break;

                    case "SERASA":
                        if (usarMock)
                        {
                            var serasaHealth = await _serasaServiceMock.HealthCheckAsync(cancellationToken);
                            status.Status = serasaHealth.Status;
                            status.Disponivel = serasaHealth.Status.ToLowerInvariant() == "OK".ToLowerInvariant();
                            status.Mensagem = $"MOCK - Verificado em {serasaHealth.Timestamp}";
                        }
                        else
                        {
                            if (_serasaServiceReal == null)
                            {
                                status.Status = "ERROR";
                                status.Disponivel = false;
                                status.Mensagem = "Serviço real SERASA não configurado";
                            }
                            else
                            {
                                var serasaHealth = await _serasaServiceReal.VerificarDisponibilidadeAsync(cancellationToken);
                                status.Status = serasaHealth.Status;
                                status.Disponivel = serasaHealth.Disponivel;
                                status.Mensagem = $"REAL - {serasaHealth.Mensagem}";
                            }
                        }
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

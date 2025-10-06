using ConsultaDocumentos.Application.DTOs.External;

namespace ConsultaDocumentos.Application.Interfaces
{
    public interface IProviderHealthCheckService
    {
        Task<ProvidersHealthCheckResponse> CheckAllProvidersAsync(CancellationToken cancellationToken = default);
        Task<ProviderHealthStatusDTO> CheckProviderAsync(string providerName, CancellationToken cancellationToken = default);
    }
}

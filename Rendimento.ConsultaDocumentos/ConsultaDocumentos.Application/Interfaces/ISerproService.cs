using ConsultaDocumentos.Application.DTOs.External.Serpro;

namespace ConsultaDocumentos.Application.Interfaces
{
    public interface ISerproService
    {
        Task<SerprocCPFResponse> ConsultarCPFAsync(string cpf, CancellationToken cancellationToken = default);
        Task<SerprocCNPJPerfil1Response> ConsultarCNPJPerfil1Async(string cnpj, CancellationToken cancellationToken = default);
        Task<SerprocCNPJPerfil2Response> ConsultarCNPJPerfil2Async(string cnpj, CancellationToken cancellationToken = default);
        Task<SerprocCNPJPerfil3Response> ConsultarCNPJPerfil3Async(string cnpj, CancellationToken cancellationToken = default);
        Task<SerproHealthCheckResponse> HealthCheckAsync(CancellationToken cancellationToken = default);
    }
}

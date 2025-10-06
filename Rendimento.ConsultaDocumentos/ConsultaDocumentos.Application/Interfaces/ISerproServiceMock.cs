using ConsultaDocumentos.Application.DTOs.External.Serpro;

namespace ConsultaDocumentos.Application.Interfaces
{
    public interface ISerproServiceMock
    {
        Task<SerprocCPFResponseMock> ConsultarCPFAsync(string cpf, CancellationToken cancellationToken = default);
        Task<SerprocCNPJPerfil1ResponseMock> ConsultarCNPJPerfil1Async(string cnpj, CancellationToken cancellationToken = default);
        Task<SerprocCNPJPerfil2ResponseMock> ConsultarCNPJPerfil2Async(string cnpj, CancellationToken cancellationToken = default);
        Task<SerprocCNPJPerfil3ResponseMock> ConsultarCNPJPerfil3Async(string cnpj, CancellationToken cancellationToken = default);
        Task<SerproHealthCheckResponseMock> HealthCheckAsync(CancellationToken cancellationToken = default);
    }
}

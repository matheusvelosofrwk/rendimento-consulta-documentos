using ConsultaDocumentos.Application.DTOs.External;

namespace ConsultaDocumentos.Application.Interfaces
{
    public interface IExternalDocumentConsultaService
    {
        Task<ConsultaDocumentoResponse> ConsultarDocumentoAsync(
            ConsultaDocumentoRequest request,
            CancellationToken cancellationToken = default);

        Task<ConsultaScoreResponse> ConsultarScoreAsync(
            ConsultaScoreRequest request,
            CancellationToken cancellationToken = default);
    }
}

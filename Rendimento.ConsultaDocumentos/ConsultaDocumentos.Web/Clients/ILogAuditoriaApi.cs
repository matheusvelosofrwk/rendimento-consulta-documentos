using ConsultaDocumentos.Web.Models;
using Refit;

namespace ConsultaDocumentos.Web.Clients
{
    public interface ILogAuditoriaApi
    {
        [Get("/LogAuditoria")]
        Task<Result<IList<LogAuditoriaViewModel>>> GetAllAsync();

        [Get("/LogAuditoria/{id}")]
        Task<Result<LogAuditoriaViewModel>> GetByIdAsync(Guid id);

        [Get("/LogAuditoria/aplicacao/{aplicacaoId}")]
        Task<Result<IList<LogAuditoriaViewModel>>> GetByAplicacaoAsync(Guid aplicacaoId);

        [Get("/LogAuditoria/documento/{documentoNumero}")]
        Task<Result<IList<LogAuditoriaViewModel>>> GetByDocumentoNumeroAsync(string documentoNumero);

        [Get("/LogAuditoria/filtrar")]
        Task<Result<IList<LogAuditoriaViewModel>>> GetWithFiltersAsync(
            [Query] DateTime? dataInicio,
            [Query] DateTime? dataFim,
            [Query] string? numeroDocumento,
            [Query] Guid? aplicacaoProvedorId,
            [Query] int? tipoDocumento);
    }
}

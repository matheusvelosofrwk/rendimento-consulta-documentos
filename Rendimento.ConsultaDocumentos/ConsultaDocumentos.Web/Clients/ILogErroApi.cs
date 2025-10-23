using ConsultaDocumentos.Web.Models;
using Refit;

namespace ConsultaDocumentos.Web.Clients
{
    public interface ILogErroApi
    {
        [Get("/LogErro")]
        Task<Result<IList<LogErroViewModel>>> GetAllAsync();

        [Get("/LogErro/{id}")]
        Task<Result<LogErroViewModel>> GetByIdAsync(Guid id);

        [Get("/LogErro/aplicacao/{aplicacao}")]
        Task<Result<IList<LogErroViewModel>>> GetByAplicacaoAsync(string aplicacao);

        [Get("/LogErro/usuario/{usuario}")]
        Task<Result<IList<LogErroViewModel>>> GetByUsuarioAsync(string usuario);

        [Get("/LogErro/filtrar")]
        Task<Result<IList<LogErroViewModel>>> GetWithFiltersAsync(
            [Query] DateTime? dataInicio,
            [Query] DateTime? dataFim,
            [Query] string? numeroDocumento,
            [Query] Guid? aplicacaoProvedorId,
            [Query] int? tipoDocumento);
    }
}

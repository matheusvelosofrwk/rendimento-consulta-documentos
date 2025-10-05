using ConsultaDocumentos.Web.Models;
using Refit;

namespace ConsultaDocumentos.Web.Clients
{
    public interface IAplicacaoProvedorApi
    {
        [Get("/AplicacaoProvedor")]
        Task<Result<IList<AplicacaoProvedorViewModel>>> GetAllAsync();

        [Get("/AplicacaoProvedor/{id}")]
        Task<Result<AplicacaoProvedorViewModel>> GetByIdAsync(Guid id);

        [Post("/AplicacaoProvedor")]
        Task<Result<AplicacaoProvedorViewModel>> CreateAsync([Body] AplicacaoProvedorViewModel model);

        [Put("/AplicacaoProvedor/{id}")]
        Task<Result<AplicacaoProvedorViewModel>> UpdateAsync(Guid id, [Body] AplicacaoProvedorViewModel model);

        [Delete("/AplicacaoProvedor/{id}")]
        Task<Result<bool>> DeleteAsync(Guid id);
    }
}

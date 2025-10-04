using ConsultaDocumentos.Web.Models;
using Refit;

namespace ConsultaDocumentos.Web.Clients
{
    public interface IAplicacaoApi
    {
        [Get("/aplicacao")]
        Task<Result<IList<AplicacaoViewModel>>> GetAllAsync();

        [Get("/aplicacao/{id}")]
        Task<Result<AplicacaoViewModel>> GetByIdAsync(Guid id);

        [Post("/aplicacao")]
        Task<Result<AplicacaoViewModel>> CreateAsync([Body] AplicacaoViewModel model);

        [Put("/aplicacao/{id}")]
        Task<Result<AplicacaoViewModel>> UpdateAsync(Guid id, [Body] AplicacaoViewModel model);

        [Delete("/aplicacao/{id}")]
        Task<Result<bool>> DeleteAsync(Guid id);
    }
}

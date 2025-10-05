using ConsultaDocumentos.Web.Models;
using Refit;

namespace ConsultaDocumentos.Web.Clients
{
    public interface IAplicacaoApi
    {
        [Get("/Aplicacao")]
        Task<Result<IList<AplicacaoViewModel>>> GetAllAsync();

        [Get("/Aplicacao/{id}")]
        Task<Result<AplicacaoViewModel>> GetByIdAsync(Guid id);

        [Post("/Aplicacao")]
        Task<Result<AplicacaoViewModel>> CreateAsync([Body] AplicacaoViewModel model);

        [Put("/Aplicacao/{id}")]
        Task<Result<AplicacaoViewModel>> UpdateAsync(Guid id, [Body] AplicacaoViewModel model);

        [Delete("/Aplicacao/{id}")]
        Task<Result<bool>> DeleteAsync(Guid id);
    }
}

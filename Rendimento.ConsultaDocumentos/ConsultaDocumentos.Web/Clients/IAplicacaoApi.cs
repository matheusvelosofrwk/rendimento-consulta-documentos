using ConsultaDocumentos.Web.Models;
using Refit;

namespace ConsultaDocumentos.Web.Clients
{
    public interface IAplicacaoApi
    {
        [Get("/aplicacao")]
        Task<IList<AplicacaoViewModel>> GetAllAsync();

        [Get("/aplicacao/{id}")]
        Task<AplicacaoViewModel> GetByIdAsync(Guid id);

        [Post("/aplicacao")]
        Task CreateAsync([Body] AplicacaoViewModel model);

        [Put("/aplicacao/{id}")]
        Task UpdateAsync(Guid id, [Body] AplicacaoViewModel model);

        [Delete("/aplicacao/{id}")]
        Task DeleteAsync(Guid id);
    }
}

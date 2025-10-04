using ConsultaDocumentos.Web.Models;
using Refit;

namespace ConsultaDocumentos.Web.Clients
{
    public interface IProvedorApi
    {
        [Get("/provedor")]
        Task<IList<ProvedorViewModel>> GetAllAsync();

        [Get("/provedor/{id}")]
        Task<ProvedorViewModel> GetByIdAsync(Guid id);

        [Post("/provedor")]
        Task CreateAsync([Body] ProvedorViewModel model);

        [Put("/provedor/{id}")]
        Task UpdateAsync(Guid id, [Body] ProvedorViewModel model);

        [Delete("/provedor/{id}")]
        Task DeleteAsync(Guid id);
    }
}

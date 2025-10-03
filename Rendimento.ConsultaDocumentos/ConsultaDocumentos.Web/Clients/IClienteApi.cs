using ConsultaDocumentos.Web.Models;
using Refit;

namespace ConsultaDocumentos.Web.Clients
{
    public interface IClienteApi
    {
        [Get("/cliente")]
        Task<IList<ClienteViewModel>> GetAllAsync();

        [Get("/cliente/{id}")]
        Task<ClienteViewModel> GetByIdAsync(Guid id);

        [Post("/cliente")]
        Task CreateAsync([Body] ClienteViewModel model);

        [Put("/cliente/{id}")]
        Task UpdateAsync(Guid id, [Body] ClienteViewModel model);

        [Delete("/cliente/{id}")]
        Task DeleteAsync(Guid id);
    }
}

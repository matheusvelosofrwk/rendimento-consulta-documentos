using ConsultaDocumentos.Web.Models;
using Refit;

namespace ConsultaDocumentos.Web.Clients
{
    public interface IClienteApi
    {
        [Get("/cliente")]
        Task<Result<IList<ClienteViewModel>>> GetAllAsync();

        [Get("/cliente/{id}")]
        Task<Result<ClienteViewModel>> GetByIdAsync(Guid id);

        [Post("/cliente")]
        Task<Result<ClienteViewModel>> CreateAsync([Body] ClienteViewModel model);

        [Put("/cliente/{id}")]
        Task<Result<ClienteViewModel>> UpdateAsync(Guid id, [Body] ClienteViewModel model);

        [Delete("/cliente/{id}")]
        Task<Result<bool>> DeleteAsync(Guid id);
    }
}

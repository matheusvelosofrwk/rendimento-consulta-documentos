using ConsultaDocumentos.Web.Models;
using Refit;

namespace ConsultaDocumentos.Web.Clients
{
    public interface IClienteApi
    {
        [Get("/Cliente")]
        Task<Result<IList<ClienteViewModel>>> GetAllAsync();

        [Get("/Cliente/{id}")]
        Task<Result<ClienteViewModel>> GetByIdAsync(Guid id);

        [Post("/Cliente")]
        Task<Result<ClienteViewModel>> CreateAsync([Body] ClienteViewModel model);

        [Put("/Cliente/{id}")]
        Task<Result<ClienteViewModel>> UpdateAsync(Guid id, [Body] ClienteViewModel model);

        [Delete("/Cliente/{id}")]
        Task<Result<bool>> DeleteAsync(Guid id);
    }
}

using ConsultaDocumentos.Web.Models;
using Refit;

namespace ConsultaDocumentos.Web.Clients
{
    public interface IProvedorApi
    {
        [Get("/Provedor")]
        Task<Result<IList<ProvedorViewModel>>> GetAllAsync();

        [Get("/Provedor/{id}")]
        Task<Result<ProvedorViewModel>> GetByIdAsync(Guid id);

        [Post("/Provedor")]
        Task<Result<ProvedorViewModel>> CreateAsync([Body] ProvedorViewModel model);

        [Put("/Provedor/{id}")]
        Task<Result<ProvedorViewModel>> UpdateAsync(Guid id, [Body] ProvedorViewModel model);

        [Delete("/Provedor/{id}")]
        Task<Result<bool>> DeleteAsync(Guid id);
    }
}

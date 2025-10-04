using ConsultaDocumentos.Web.Models;
using Refit;

namespace ConsultaDocumentos.Web.Clients
{
    public interface IProvedorApi
    {
        [Get("/provedor")]
        Task<Result<IList<ProvedorViewModel>>> GetAllAsync();

        [Get("/provedor/{id}")]
        Task<Result<ProvedorViewModel>> GetByIdAsync(Guid id);

        [Post("/provedor")]
        Task<Result<ProvedorViewModel>> CreateAsync([Body] ProvedorViewModel model);

        [Put("/provedor/{id}")]
        Task<Result<ProvedorViewModel>> UpdateAsync(Guid id, [Body] ProvedorViewModel model);

        [Delete("/provedor/{id}")]
        Task<Result<bool>> DeleteAsync(Guid id);
    }
}

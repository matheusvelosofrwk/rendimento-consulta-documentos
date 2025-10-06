using ConsultaDocumentos.Web.Models;
using Refit;

namespace ConsultaDocumentos.Web.Clients
{
    public interface IQuadroSocietarioApi
    {
        [Get("/QuadroSocietario")]
        Task<Result<IList<QuadroSocietarioViewModel>>> GetAllAsync();

        [Get("/QuadroSocietario/{id}")]
        Task<Result<QuadroSocietarioViewModel>> GetByIdAsync(Guid id);

        [Post("/QuadroSocietario")]
        Task<Result<QuadroSocietarioViewModel>> CreateAsync([Body] QuadroSocietarioViewModel model);

        [Put("/QuadroSocietario/{id}")]
        Task<Result<QuadroSocietarioViewModel>> UpdateAsync(Guid id, [Body] QuadroSocietarioViewModel model);

        [Delete("/QuadroSocietario/{id}")]
        Task<Result<bool>> DeleteAsync(Guid id);
    }
}

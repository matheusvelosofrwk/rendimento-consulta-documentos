using ConsultaDocumentos.Web.Models;
using Refit;

namespace ConsultaDocumentos.Web.Clients
{
    public interface IRoleApi
    {
        [Get("/Role")]
        Task<Result<IEnumerable<RoleViewModel>>> GetAllAsync();

        [Get("/Role/{id}")]
        Task<Result<RoleViewModel>> GetByIdAsync(string id);

        [Post("/Role")]
        Task<Result<RoleViewModel>> CreateAsync([Body] CreateRoleViewModel model);

        [Put("/Role/{id}")]
        Task<Result<RoleViewModel>> UpdateAsync(string id, [Body] UpdateRoleViewModel model);

        [Delete("/Role/{id}")]
        Task<Result<bool>> DeleteAsync(string id);
    }
}

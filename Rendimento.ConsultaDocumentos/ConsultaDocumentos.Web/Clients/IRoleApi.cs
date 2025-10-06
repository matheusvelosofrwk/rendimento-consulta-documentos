using ConsultaDocumentos.Web.Models;
using Refit;

namespace ConsultaDocumentos.Web.Clients
{
    public interface IRoleApi
    {
        [Get("/role")]
        Task<Result<IEnumerable<RoleViewModel>>> GetAllAsync();

        [Get("/role/{id}")]
        Task<Result<RoleViewModel>> GetByIdAsync(string id);

        [Post("/role")]
        Task<Result<RoleViewModel>> CreateAsync([Body] CreateRoleViewModel model);

        [Put("/role/{id}")]
        Task<Result<RoleViewModel>> UpdateAsync(string id, [Body] UpdateRoleViewModel model);

        [Delete("/role/{id}")]
        Task<Result<bool>> DeleteAsync(string id);
    }
}

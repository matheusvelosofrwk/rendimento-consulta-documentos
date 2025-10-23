using ConsultaDocumentos.Web.Models;
using Refit;

namespace ConsultaDocumentos.Web.Clients
{
    public interface IUserApi
    {
        [Get("/User")]
        Task<Result<IEnumerable<UserViewModel>>> GetAllAsync();

        [Get("/User/{id}")]
        Task<Result<UserViewModel>> GetByIdAsync(string id);

        [Post("/User")]
        Task<Result<UserViewModel>> CreateAsync([Body] CreateUserViewModel model);

        [Put("/User/{id}")]
        Task<Result<UserViewModel>> UpdateAsync(string id, [Body] UpdateUserViewModel model);

        [Delete("/User/{id}")]
        Task<Result<bool>> DeleteAsync(string id);

        [Put("/User/{id}/roles")]
        Task<Result<bool>> ManageRolesAsync(string id, [Body] ManageUserRolesViewModel model);
    }
}

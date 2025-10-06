using ConsultaDocumentos.Web.Models;
using Refit;

namespace ConsultaDocumentos.Web.Clients
{
    public interface IUserApi
    {
        [Get("/user")]
        Task<Result<IEnumerable<UserViewModel>>> GetAllAsync();

        [Get("/user/{id}")]
        Task<Result<UserViewModel>> GetByIdAsync(string id);

        [Post("/user")]
        Task<Result<UserViewModel>> CreateAsync([Body] CreateUserViewModel model);

        [Put("/user/{id}")]
        Task<Result<UserViewModel>> UpdateAsync(string id, [Body] UpdateUserViewModel model);

        [Delete("/user/{id}")]
        Task<Result<bool>> DeleteAsync(string id);

        [Put("/user/{id}/roles")]
        Task<Result<bool>> ManageRolesAsync(string id, [Body] ManageUserRolesViewModel model);
    }
}

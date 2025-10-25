using ConsultaDocumentos.Web.Models;

namespace ConsultaDocumentos.Web.Services.Http
{
    public class UserHttpService : BaseHttpService
    {
        public UserHttpService(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<Result<IList<UserViewModel>>> GetAllAsync()
        {
            return await GetAsync<IList<UserViewModel>>("User");
        }

        public async Task<Result<UserViewModel>> GetByIdAsync(string id)
        {
            return await GetAsync<UserViewModel>($"User/{id}");
        }

        public async Task<Result<UserViewModel>> CreateAsync(CreateUserViewModel model)
        {
            return await PostAsync<UserViewModel>("User", model);
        }

        public async Task<Result<UserViewModel>> UpdateAsync(string id, UpdateUserViewModel model)
        {
            return await PutAsync<UserViewModel>($"User/{id}", model);
        }

        public async Task<Result<bool>> DeleteAsync(string id)
        {
            return await DeleteAsync<bool>($"User/{id}");
        }

        public async Task<Result<IList<string>>> GetUserRolesAsync(string userId)
        {
            return await GetAsync<IList<string>>($"User/{userId}/roles");
        }

        public async Task<Result<bool>> ManageUserRolesAsync(string userId, ManageUserRolesViewModel model)
        {
            return await PostAsync<bool>($"User/{userId}/roles", model);
        }

        public async Task<Result<bool>> ManageRolesAsync(string userId, ManageUserRolesViewModel model)
        {
            return await PostAsync<bool>($"User/{userId}/roles", model);
        }
    }
}

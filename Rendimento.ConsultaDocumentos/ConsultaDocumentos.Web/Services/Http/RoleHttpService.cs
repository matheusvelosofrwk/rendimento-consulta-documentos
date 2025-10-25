using ConsultaDocumentos.Web.Models;

namespace ConsultaDocumentos.Web.Services.Http
{
    public class RoleHttpService : BaseHttpService
    {
        public RoleHttpService(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<Result<IList<RoleViewModel>>> GetAllAsync()
        {
            return await GetAsync<IList<RoleViewModel>>("Role");
        }

        public async Task<Result<RoleViewModel>> GetByIdAsync(string id)
        {
            return await GetAsync<RoleViewModel>($"Role/{id}");
        }

        public async Task<Result<RoleViewModel>> CreateAsync(CreateRoleViewModel model)
        {
            return await PostAsync<RoleViewModel>("Role", model);
        }

        public async Task<Result<RoleViewModel>> UpdateAsync(string id, UpdateRoleViewModel model)
        {
            return await PutAsync<RoleViewModel>($"Role/{id}", model);
        }

        public async Task<Result<bool>> DeleteAsync(string id)
        {
            return await DeleteAsync<bool>($"Role/{id}");
        }
    }
}

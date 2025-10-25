using ConsultaDocumentos.Web.Models;

namespace ConsultaDocumentos.Web.Services.Http
{
    public class ProvedorHttpService : BaseHttpService
    {
        public ProvedorHttpService(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<Result<IList<ProvedorViewModel>>> GetAllAsync()
        {
            return await GetAsync<IList<ProvedorViewModel>>("Provedor");
        }

        public async Task<Result<ProvedorViewModel>> GetByIdAsync(Guid id)
        {
            return await GetAsync<ProvedorViewModel>($"Provedor/{id}");
        }

        public async Task<Result<ProvedorViewModel>> CreateAsync(ProvedorViewModel model)
        {
            return await PostAsync<ProvedorViewModel>("Provedor", model);
        }

        public async Task<Result<ProvedorViewModel>> UpdateAsync(Guid id, ProvedorViewModel model)
        {
            return await PutAsync<ProvedorViewModel>($"Provedor/{id}", model);
        }

        public async Task<Result<bool>> DeleteAsync(Guid id)
        {
            return await DeleteAsync<bool>($"Provedor/{id}");
        }
    }
}

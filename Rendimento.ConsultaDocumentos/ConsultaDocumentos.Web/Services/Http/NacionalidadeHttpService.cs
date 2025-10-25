using ConsultaDocumentos.Web.Models;

namespace ConsultaDocumentos.Web.Services.Http
{
    public class NacionalidadeHttpService : BaseHttpService
    {
        public NacionalidadeHttpService(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<Result<IList<NacionalidadeViewModel>>> GetAllAsync()
        {
            return await GetAsync<IList<NacionalidadeViewModel>>("Nacionalidade");
        }

        public async Task<Result<NacionalidadeViewModel>> GetByIdAsync(Guid id)
        {
            return await GetAsync<NacionalidadeViewModel>($"Nacionalidade/{id}");
        }

        public async Task<Result<NacionalidadeViewModel>> CreateAsync(NacionalidadeViewModel model)
        {
            return await PostAsync<NacionalidadeViewModel>("Nacionalidade", model);
        }

        public async Task<Result<NacionalidadeViewModel>> UpdateAsync(Guid id, NacionalidadeViewModel model)
        {
            return await PutAsync<NacionalidadeViewModel>($"Nacionalidade/{id}", model);
        }

        public async Task<Result<bool>> DeleteAsync(Guid id)
        {
            return await DeleteAsync<bool>($"Nacionalidade/{id}");
        }
    }
}

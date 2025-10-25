using ConsultaDocumentos.Web.Models;

namespace ConsultaDocumentos.Web.Services.Http
{
    public class AplicacaoHttpService : BaseHttpService
    {
        public AplicacaoHttpService(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<Result<IList<AplicacaoViewModel>>> GetAllAsync()
        {
            return await GetAsync<IList<AplicacaoViewModel>>("Aplicacao");
        }

        public async Task<Result<AplicacaoViewModel>> GetByIdAsync(Guid id)
        {
            return await GetAsync<AplicacaoViewModel>($"Aplicacao/{id}");
        }

        public async Task<Result<AplicacaoViewModel>> CreateAsync(AplicacaoViewModel model)
        {
            return await PostAsync<AplicacaoViewModel>("Aplicacao", model);
        }

        public async Task<Result<AplicacaoViewModel>> UpdateAsync(Guid id, AplicacaoViewModel model)
        {
            return await PutAsync<AplicacaoViewModel>($"Aplicacao/{id}", model);
        }

        public async Task<Result<bool>> DeleteAsync(Guid id)
        {
            return await DeleteAsync<bool>($"Aplicacao/{id}");
        }
    }
}

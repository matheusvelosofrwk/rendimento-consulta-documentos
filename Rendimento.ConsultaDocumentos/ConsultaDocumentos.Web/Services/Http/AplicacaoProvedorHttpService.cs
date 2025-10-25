using ConsultaDocumentos.Web.Models;

namespace ConsultaDocumentos.Web.Services.Http
{
    public class AplicacaoProvedorHttpService : BaseHttpService
    {
        public AplicacaoProvedorHttpService(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<Result<IList<AplicacaoProvedorViewModel>>> GetAllAsync()
        {
            return await GetAsync<IList<AplicacaoProvedorViewModel>>("AplicacaoProvedor");
        }

        public async Task<Result<IList<AplicacaoProvedorViewModel>>> GetByAplicacaoIdAsync(Guid aplicacaoId)
        {
            return await GetAsync<IList<AplicacaoProvedorViewModel>>($"AplicacaoProvedor/aplicacao/{aplicacaoId}");
        }

        public async Task<Result<AplicacaoProvedorViewModel>> GetByIdAsync(Guid id)
        {
            return await GetAsync<AplicacaoProvedorViewModel>($"AplicacaoProvedor/{id}");
        }

        public async Task<Result<AplicacaoProvedorViewModel>> CreateAsync(AplicacaoProvedorViewModel model)
        {
            return await PostAsync<AplicacaoProvedorViewModel>("AplicacaoProvedor", model);
        }

        public async Task<Result<AplicacaoProvedorViewModel>> UpdateAsync(Guid id, AplicacaoProvedorViewModel model)
        {
            return await PutAsync<AplicacaoProvedorViewModel>($"AplicacaoProvedor/{id}", model);
        }

        public async Task<Result<bool>> DeleteAsync(Guid id)
        {
            return await DeleteAsync<bool>($"AplicacaoProvedor/{id}");
        }
    }
}

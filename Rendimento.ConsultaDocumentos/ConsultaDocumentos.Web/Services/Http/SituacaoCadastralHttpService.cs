using ConsultaDocumentos.Web.Models;

namespace ConsultaDocumentos.Web.Services.Http
{
    public class SituacaoCadastralHttpService : BaseHttpService
    {
        public SituacaoCadastralHttpService(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<Result<IList<SituacaoCadastralViewModel>>> GetAllAsync()
        {
            return await GetAsync<IList<SituacaoCadastralViewModel>>("SituacaoCadastral");
        }

        public async Task<Result<SituacaoCadastralViewModel>> GetByIdAsync(Guid id)
        {
            return await GetAsync<SituacaoCadastralViewModel>($"SituacaoCadastral/{id}");
        }

        public async Task<Result<SituacaoCadastralViewModel>> CreateAsync(SituacaoCadastralViewModel model)
        {
            return await PostAsync<SituacaoCadastralViewModel>("SituacaoCadastral", model);
        }

        public async Task<Result<SituacaoCadastralViewModel>> UpdateAsync(Guid id, SituacaoCadastralViewModel model)
        {
            return await PutAsync<SituacaoCadastralViewModel>($"SituacaoCadastral/{id}", model);
        }

        public async Task<Result<bool>> DeleteAsync(Guid id)
        {
            return await DeleteAsync<bool>($"SituacaoCadastral/{id}");
        }

        public async Task<Result<IList<SituacaoCadastralViewModel>>> GetByTipoPessoaAsync(char tipoPessoa)
        {
            return await GetAsync<IList<SituacaoCadastralViewModel>>($"SituacaoCadastral/tipopessoa/{tipoPessoa}");
        }
    }
}

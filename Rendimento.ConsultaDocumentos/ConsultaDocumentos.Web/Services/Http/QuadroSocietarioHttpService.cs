using ConsultaDocumentos.Web.Models;

namespace ConsultaDocumentos.Web.Services.Http
{
    public class QuadroSocietarioHttpService : BaseHttpService
    {
        public QuadroSocietarioHttpService(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<Result<IList<QuadroSocietarioViewModel>>> GetAllAsync()
        {
            return await GetAsync<IList<QuadroSocietarioViewModel>>("QuadroSocietario");
        }

        public async Task<Result<QuadroSocietarioViewModel>> GetByIdAsync(Guid id)
        {
            return await GetAsync<QuadroSocietarioViewModel>($"QuadroSocietario/{id}");
        }

        public async Task<Result<IList<QuadroSocietarioViewModel>>> GetByDocumentoIdAsync(Guid documentoId)
        {
            return await GetAsync<IList<QuadroSocietarioViewModel>>($"QuadroSocietario/documento/{documentoId}");
        }

        public async Task<Result<QuadroSocietarioViewModel>> CreateAsync(QuadroSocietarioViewModel model)
        {
            return await PostAsync<QuadroSocietarioViewModel>("QuadroSocietario", model);
        }

        public async Task<Result<QuadroSocietarioViewModel>> UpdateAsync(Guid id, QuadroSocietarioViewModel model)
        {
            return await PutAsync<QuadroSocietarioViewModel>($"QuadroSocietario/{id}", model);
        }

        public async Task<Result<bool>> DeleteAsync(Guid id)
        {
            return await DeleteAsync<bool>($"QuadroSocietario/{id}");
        }
    }
}

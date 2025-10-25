using ConsultaDocumentos.Web.Models;

namespace ConsultaDocumentos.Web.Services.Http
{
    public class AuthHttpService : BaseHttpService
    {
        public AuthHttpService(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<Result<AuthResponseViewModel>> LoginAsync(LoginViewModel model)
        {
            return await PostAsync<AuthResponseViewModel>("auth/login", model);
        }

        public async Task<Result<AuthResponseViewModel>> RegisterAsync(RegisterViewModel model)
        {
            return await PostAsync<AuthResponseViewModel>("auth/register", model);
        }
    }
}

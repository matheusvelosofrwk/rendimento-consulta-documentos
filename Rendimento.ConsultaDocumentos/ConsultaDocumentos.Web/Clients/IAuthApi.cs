using ConsultaDocumentos.Web.Models;
using Refit;

namespace ConsultaDocumentos.Web.Clients
{
    public interface IAuthApi
    {
        [Post("/auth/login")]
        Task<Result<AuthResponseViewModel>> LoginAsync([Body] LoginViewModel model);

        [Post("/auth/register")]
        Task<Result<AuthResponseViewModel>> RegisterAsync([Body] RegisterViewModel model);
    }
}

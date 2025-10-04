using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;

namespace ConsultaDocumentos.Web.Handlers
{
    public class AuthDelegatingHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthDelegatingHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Obter o token do contexto de autenticação (cookie)
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext != null)
            {
                var token = await httpContext.GetTokenAsync("access_token");

                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}

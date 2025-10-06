using Microsoft.Extensions.Configuration;
using System;

namespace ConsultaDocumentos.Application.Services.External
{
    /// <summary>
    /// Serviço para selecionar entre provedores Mock e Real
    /// </summary>
    public interface IProviderSelector
    {
        bool UsarMock();
        string ObterModoAtual();
    }

    public class ProviderSelector : IProviderSelector
    {
        private readonly IConfiguration _configuration;

        public ProviderSelector(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool UsarMock()
        {
            // Lê a configuração UseMockProviders
            // Default: true (para manter compatibilidade)
            var valor = _configuration["ExternalProviders:UseMockProviders"];
            if (string.IsNullOrEmpty(valor))
                return true;

            return bool.TryParse(valor, out var result) ? result : true;
        }

        public string ObterModoAtual()
        {
            return UsarMock() ? "MOCK" : "REAL";
        }
    }
}

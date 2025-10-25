using ConsultaDocumentos.Web.Models;

namespace ConsultaDocumentos.Web.Services.Http
{
    public class LogErroHttpService : BaseHttpService
    {
        public LogErroHttpService(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<Result<IList<LogErroViewModel>>> GetAllAsync()
        {
            return await GetAsync<IList<LogErroViewModel>>("LogErro");
        }

        public async Task<Result<LogErroViewModel>> GetByIdAsync(Guid id)
        {
            return await GetAsync<LogErroViewModel>($"LogErro/{id}");
        }

        public async Task<Result<IList<LogErroViewModel>>> GetByFiltrosAsync(LogErroFiltrosViewModel filtros)
        {
            var queryString = BuildQueryString(filtros);
            return await GetAsync<IList<LogErroViewModel>>($"LogErro/filtrar{queryString}");
        }

        private string BuildQueryString(LogErroFiltrosViewModel filtros)
        {
            var queryParams = new List<string>();

            if (filtros.DataInicio.HasValue)
                queryParams.Add($"dataInicio={filtros.DataInicio:yyyy-MM-dd}");

            if (filtros.DataFim.HasValue)
                queryParams.Add($"dataFim={filtros.DataFim:yyyy-MM-dd}");

            if (!string.IsNullOrEmpty(filtros.NumeroDocumento))
                queryParams.Add($"numeroDocumento={Uri.EscapeDataString(filtros.NumeroDocumento)}");

            if (filtros.AplicacaoProvedorId.HasValue)
                queryParams.Add($"aplicacaoProvedorId={filtros.AplicacaoProvedorId}");

            if (filtros.TipoDocumento.HasValue)
                queryParams.Add($"tipoDocumento={filtros.TipoDocumento}");

            return queryParams.Any() ? "?" + string.Join("&", queryParams) : string.Empty;
        }
    }
}

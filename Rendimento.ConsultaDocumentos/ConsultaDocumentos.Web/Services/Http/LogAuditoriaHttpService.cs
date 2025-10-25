using ConsultaDocumentos.Web.Models;

namespace ConsultaDocumentos.Web.Services.Http
{
    public class LogAuditoriaHttpService : BaseHttpService
    {
        public LogAuditoriaHttpService(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<Result<IList<LogAuditoriaViewModel>>> GetAllAsync()
        {
            return await GetAsync<IList<LogAuditoriaViewModel>>("LogAuditoria");
        }

        public async Task<Result<LogAuditoriaViewModel>> GetByIdAsync(Guid id)
        {
            return await GetAsync<LogAuditoriaViewModel>($"LogAuditoria/{id}");
        }

        public async Task<Result<IList<LogAuditoriaViewModel>>> GetByFiltrosAsync(LogAuditoriaFiltrosViewModel filtros)
        {
            var queryString = BuildQueryString(filtros);
            return await GetAsync<IList<LogAuditoriaViewModel>>($"LogAuditoria/filtrar{queryString}");
        }

        private string BuildQueryString(LogAuditoriaFiltrosViewModel filtros)
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

using ConsultaDocumentos.Web.Models;

namespace ConsultaDocumentos.Web.Services.Http
{
    public class DocumentoHttpService : BaseHttpService
    {
        public DocumentoHttpService(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<Result<IList<DocumentoViewModel>>> GetAllAsync()
        {
            return await GetAsync<IList<DocumentoViewModel>>("Documento");
        }

        public async Task<Result<DocumentoViewModel>> GetByIdAsync(Guid id)
        {
            return await GetAsync<DocumentoViewModel>($"Documento/{id}");
        }

        public async Task<Result<ConsultaDocumentoResultViewModel>> ConsultarAsync(ConsultaDocumentoViewModel model)
        {
            return await PostAsync<ConsultaDocumentoResultViewModel>("Documento/consultar", model);
        }

        public async Task<Result<ConsultaDocumentoResultViewModel>> ConsultarCPFAsync(
            string numeroDocumento,
            Guid aplicacaoId,
            string? tipoConsulta = null,
            string? origemConsulta = null,
            bool? consultarVencidos = null)
        {
            var url = $"Documento/consultar/cpf?numeroDocumento={Uri.EscapeDataString(numeroDocumento)}&aplicacaoId={aplicacaoId}";

            if (!string.IsNullOrEmpty(tipoConsulta))
                url += $"&tipoConsulta={Uri.EscapeDataString(tipoConsulta)}";

            if (!string.IsNullOrEmpty(origemConsulta))
                url += $"&origemConsulta={Uri.EscapeDataString(origemConsulta)}";

            if (consultarVencidos.HasValue)
                url += $"&consultarVencidos={consultarVencidos.Value}";

            return await GetAsync<ConsultaDocumentoResultViewModel>(url);
        }

        public async Task<Result<ConsultaDocumentoResultViewModel>> ConsultarCNPJAsync(
            string numeroDocumento,
            Guid aplicacaoId,
            string? perfilCNPJ = null,
            string? tipoConsulta = null,
            string? origemConsulta = null,
            bool? consultarVencidos = null)
        {
            var url = $"Documento/consultar/cnpj?numeroDocumento={Uri.EscapeDataString(numeroDocumento)}&aplicacaoId={aplicacaoId}";

            if (!string.IsNullOrEmpty(perfilCNPJ))
                url += $"&perfilCNPJ={Uri.EscapeDataString(perfilCNPJ)}";

            if (!string.IsNullOrEmpty(tipoConsulta))
                url += $"&tipoConsulta={Uri.EscapeDataString(tipoConsulta)}";

            if (!string.IsNullOrEmpty(origemConsulta))
                url += $"&origemConsulta={Uri.EscapeDataString(origemConsulta)}";

            if (consultarVencidos.HasValue)
                url += $"&consultarVencidos={consultarVencidos.Value}";

            return await GetAsync<ConsultaDocumentoResultViewModel>(url);
        }

        public async Task<Result<DocumentoViewModel>> CreateAsync(DocumentoViewModel model)
        {
            return await PostAsync<DocumentoViewModel>("Documento", model);
        }

        public async Task<Result<DocumentoViewModel>> UpdateAsync(Guid id, DocumentoViewModel model)
        {
            return await PutAsync<DocumentoViewModel>($"Documento/{id}", model);
        }

        public async Task<Result<bool>> DeleteAsync(Guid id)
        {
            return await DeleteAsync<bool>($"Documento/{id}");
        }
    }
}

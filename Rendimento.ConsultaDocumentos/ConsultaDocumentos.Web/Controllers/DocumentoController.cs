using ConsultaDocumentos.Web.Clients;
using ConsultaDocumentos.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ConsultaDocumentos.Web.Controllers
{
    public class DocumentoController : Controller
    {
        private readonly IDocumentoApi _documentoApi;
        private readonly IAplicacaoApi _aplicacaoApi;

        public DocumentoController(IDocumentoApi documentoApi, IAplicacaoApi aplicacaoApi)
        {
            _documentoApi = documentoApi;
            _aplicacaoApi = aplicacaoApi;
        }

        // GET: Documento/Consultar
        public async Task<IActionResult> Consultar()
        {
            await CarregarAplicacoesAsync();
            return View(new ConsultaDocumentoViewModel());
        }

        // POST: Documento/Consultar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Consultar(ConsultaDocumentoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await CarregarAplicacoesAsync();
                return View(model);
            }

            try
            {
                ConsultaDocumentoResultViewModel resultado;

                if (model.TipoDocumento == "CPF")
                {
                    resultado = await _documentoApi.ConsultarCPFAsync(model.NumeroDocumento, model.AplicacaoId);
                }
                else
                {
                    resultado = await _documentoApi.ConsultarCNPJAsync(model.NumeroDocumento, model.AplicacaoId, model.PerfilCNPJ);
                }

                ViewBag.Resultado = resultado;
                await CarregarAplicacoesAsync();
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Erro ao consultar documento: {ex.Message}");
                await CarregarAplicacoesAsync();
                return View(model);
            }
        }

        private async Task CarregarAplicacoesAsync()
        {
            try
            {
                var aplicacoesResult = await _aplicacaoApi.GetAllAsync();
                if (aplicacoesResult.Success && aplicacoesResult.Data != null)
                {
                    ViewBag.Aplicacoes = new SelectList(aplicacoesResult.Data, "Id", "Nome");
                }
                else
                {
                    ViewBag.Aplicacoes = new SelectList(Enumerable.Empty<SelectListItem>());
                }
            }
            catch
            {
                ViewBag.Aplicacoes = new SelectList(Enumerable.Empty<SelectListItem>());
            }
        }
    }
}

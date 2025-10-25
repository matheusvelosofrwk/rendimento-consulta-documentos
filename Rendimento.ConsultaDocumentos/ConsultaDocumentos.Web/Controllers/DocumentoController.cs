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
                ConsultaDocumentoResultViewModel? resultado = null;

                if (model.TipoDocumento == "CPF")
                {
                    resultado = await _documentoApi.ConsultarCPFAsync(
                        model.NumeroDocumento,
                        model.AplicacaoId,
                        model.TipoConsulta,
                        model.OrigemConsulta,
                        model.ConsultarVencidos);
                }
                else
                {
                    resultado = await _documentoApi.ConsultarCNPJAsync(
                        model.NumeroDocumento,
                        model.AplicacaoId,
                        model.PerfilCNPJ,
                        model.TipoConsulta,
                        model.OrigemConsulta,
                        model.ConsultarVencidos);
                }

                // Garantir que resultado tenha valores padrão se vier null
                if (resultado == null)
                {
                    resultado = new ConsultaDocumentoResultViewModel
                    {
                        Sucesso = false,
                        Mensagem = "Nenhuma resposta recebida do servidor",
                        Erro = "O servidor não retornou uma resposta válida"
                    };
                }

                ViewBag.Resultado = resultado;
                await CarregarAplicacoesAsync();
                return View(model);
            }
            catch (Refit.ApiException apiEx)
            {
                // Erro da API
                var errorMessage = $"Erro na API: {apiEx.Message}";
                if (apiEx.HasContent)
                {
                    errorMessage += $" - Detalhes: {apiEx.Content}";
                }

                ViewBag.Resultado = new ConsultaDocumentoResultViewModel
                {
                    Sucesso = false,
                    Mensagem = "Erro ao consultar documento",
                    Erro = errorMessage
                };

                await CarregarAplicacoesAsync();
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Resultado = new ConsultaDocumentoResultViewModel
                {
                    Sucesso = false,
                    Mensagem = "Erro inesperado ao consultar documento",
                    Erro = ex.Message
                };

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

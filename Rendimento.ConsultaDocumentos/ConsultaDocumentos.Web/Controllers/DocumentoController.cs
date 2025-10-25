using ConsultaDocumentos.Web.Services.Http;
using ConsultaDocumentos.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ConsultaDocumentos.Web.Controllers
{
    public class DocumentoController : Controller
    {
        private readonly DocumentoHttpService _documentoService;
        private readonly AplicacaoHttpService _aplicacaoService;

        public DocumentoController(DocumentoHttpService documentoService, AplicacaoHttpService aplicacaoService)
        {
            _documentoService = documentoService;
            _aplicacaoService = aplicacaoService;
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
                Result<ConsultaDocumentoResultViewModel>? resultWrapper = null;

                if (model.TipoDocumento == "CPF")
                {
                    resultWrapper = await _documentoService.ConsultarCPFAsync(
                        model.NumeroDocumento,
                        model.AplicacaoId,
                        model.TipoConsulta.ToString(),
                        model.OrigemConsulta.ToString(),
                        model.ConsultarVencidos);
                }
                else
                {
                    resultWrapper = await _documentoService.ConsultarCNPJAsync(
                        model.NumeroDocumento,
                        model.AplicacaoId,
                        model.PerfilCNPJ.ToString(),
                        model.TipoConsulta.ToString(),
                        model.OrigemConsulta.ToString(),
                        model.ConsultarVencidos);
                }

                ConsultaDocumentoResultViewModel? resultado = resultWrapper?.Data;

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
            catch (HttpRequestException httpEx)
            {
                // Erro da API
                var errorMessage = $"Erro ao comunicar com a API: {httpEx.Message}";

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
                var aplicacoesResult = await _aplicacaoService.GetAllAsync();
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

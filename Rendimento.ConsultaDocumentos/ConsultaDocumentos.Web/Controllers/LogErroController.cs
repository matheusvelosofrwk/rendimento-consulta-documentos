using ConsultaDocumentos.Web.Clients;
using ConsultaDocumentos.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ConsultaDocumentos.Web.Controllers
{
    [Authorize]
    public class LogErroController : Controller
    {
        private readonly ILogErroApi _logErroApi;
        private readonly IAplicacaoApi _aplicacaoApi;

        public LogErroController(ILogErroApi logErroApi, IAplicacaoApi aplicacaoApi)
        {
            _logErroApi = logErroApi;
            _aplicacaoApi = aplicacaoApi;
        }

        public async Task<IActionResult> Index(LogErroFiltrosViewModel? filtros)
        {
            try
            {
                // Carregar aplicações para o dropdown
                await CarregarListasDropdown();

                Result<IList<LogErroViewModel>> result;

                // Se há filtros, usar endpoint de filtro
                if (filtros != null && (filtros.DataInicio.HasValue || filtros.DataFim.HasValue ||
                    !string.IsNullOrWhiteSpace(filtros.NumeroDocumento) || filtros.AplicacaoProvedorId.HasValue ||
                    filtros.TipoDocumento.HasValue))
                {
                    result = await _logErroApi.GetWithFiltersAsync(
                        filtros.DataInicio,
                        filtros.DataFim,
                        filtros.NumeroDocumento,
                        filtros.AplicacaoProvedorId,
                        filtros.TipoDocumento);
                }
                else
                {
                    result = await _logErroApi.GetAllAsync();
                }

                if (!result.Success)
                {
                    TempData["Error"] = result.Notifications.Any()
                        ? string.Join(", ", result.Notifications)
                        : "Erro ao carregar logs de erro.";
                    return View(new List<Models.LogErroViewModel>());
                }

                return View(result.Data);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Erro ao carregar logs de erro: {ex.Message}";
                return View(new List<Models.LogErroViewModel>());
            }
        }

        private async Task CarregarListasDropdown()
        {
            try
            {
                // Carregar aplicações
                var aplicacoesResult = await _aplicacaoApi.GetAllAsync();
                if (aplicacoesResult.Success && aplicacoesResult.Data != null)
                {
                    ViewBag.Aplicacoes = new SelectList(aplicacoesResult.Data, "Id", "Nome");
                }
                else
                {
                    ViewBag.Aplicacoes = new SelectList(Enumerable.Empty<SelectListItem>());
                }

                // Lista de tipos de documento (CPF = 1, CNPJ = 2)
                ViewBag.TiposDocumento = new SelectList(new[]
                {
                    new { Id = 1, Nome = "CPF" },
                    new { Id = 2, Nome = "CNPJ" }
                }, "Id", "Nome");
            }
            catch
            {
                ViewBag.Aplicacoes = new SelectList(Enumerable.Empty<SelectListItem>());
                ViewBag.TiposDocumento = new SelectList(Enumerable.Empty<SelectListItem>());
            }
        }

        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var result = await _logErroApi.GetByIdAsync(id);

                if (!result.Success)
                {
                    TempData["Error"] = result.Notifications.Any()
                        ? string.Join(", ", result.Notifications)
                        : "Erro ao carregar detalhes do log.";
                    return RedirectToAction(nameof(Index));
                }

                return View(result.Data);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Erro ao carregar detalhes do log: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}

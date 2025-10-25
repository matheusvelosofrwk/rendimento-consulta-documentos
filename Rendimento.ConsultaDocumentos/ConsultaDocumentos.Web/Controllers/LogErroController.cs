using ConsultaDocumentos.Web.Services.Http;
using ConsultaDocumentos.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ConsultaDocumentos.Web.Controllers
{
    [Authorize]
    public class LogErroController : Controller
    {
        private readonly LogErroHttpService _logErroService;
        private readonly AplicacaoHttpService _aplicacaoService;

        public LogErroController(LogErroHttpService logErroService, AplicacaoHttpService aplicacaoService)
        {
            _logErroService = logErroService;
            _aplicacaoService = aplicacaoService;
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
                    result = await _logErroService.GetByFiltrosAsync(filtros);
                }
                else
                {
                    result = await _logErroService.GetAllAsync();
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
                var aplicacoesResult = await _aplicacaoService.GetAllAsync();
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
                var result = await _logErroService.GetByIdAsync(id);

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

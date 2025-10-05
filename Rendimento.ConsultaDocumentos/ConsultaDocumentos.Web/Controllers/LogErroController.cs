using ConsultaDocumentos.Web.Clients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConsultaDocumentos.Web.Controllers
{
    [Authorize]
    public class LogErroController : Controller
    {
        private readonly ILogErroApi _logErroApi;

        public LogErroController(ILogErroApi logErroApi)
        {
            _logErroApi = logErroApi;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var result = await _logErroApi.GetAllAsync();

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

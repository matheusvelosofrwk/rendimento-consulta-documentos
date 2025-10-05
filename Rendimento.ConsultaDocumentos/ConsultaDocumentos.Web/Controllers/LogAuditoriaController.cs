using ConsultaDocumentos.Web.Clients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConsultaDocumentos.Web.Controllers
{
    [Authorize]
    public class LogAuditoriaController : Controller
    {
        private readonly ILogAuditoriaApi _logAuditoriaApi;

        public LogAuditoriaController(ILogAuditoriaApi logAuditoriaApi)
        {
            _logAuditoriaApi = logAuditoriaApi;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var result = await _logAuditoriaApi.GetAllAsync();

                if (!result.Success)
                {
                    TempData["Error"] = result.Notifications.Any()
                        ? string.Join(", ", result.Notifications)
                        : "Erro ao carregar logs de auditoria.";
                    return View(new List<Models.LogAuditoriaViewModel>());
                }

                return View(result.Data);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Erro ao carregar logs de auditoria: {ex.Message}";
                return View(new List<Models.LogAuditoriaViewModel>());
            }
        }

        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var result = await _logAuditoriaApi.GetByIdAsync(id);

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

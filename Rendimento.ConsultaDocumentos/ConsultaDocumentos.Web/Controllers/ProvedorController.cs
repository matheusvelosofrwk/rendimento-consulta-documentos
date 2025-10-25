using ConsultaDocumentos.Web.Models;
using ConsultaDocumentos.Web.Services.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ConsultaDocumentos.Web.Controllers
{
    [Authorize]
    public class ProvedorController : Controller
    {
        private readonly ProvedorHttpService _provedorService;

        public ProvedorController(ProvedorHttpService provedorService)
        {
            _provedorService = provedorService;
        }

        // GET: ProvedorController
        public async Task<ActionResult> Index()
        {
            var result = await _provedorService.GetAllAsync();

            if (!result.Success)
            {
                foreach (var notification in result.Notifications)
                {
                    ModelState.AddModelError(string.Empty, notification);
                }
                return View(new List<ProvedorViewModel>());
            }

            return View(result.Data);
        }

        // GET: ProvedorController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProvedorController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync(ProvedorViewModel model)
        {
            var result = await _provedorService.CreateAsync(model);

            if (!result.Success)
            {
                foreach (var notification in result.Notifications)
                {
                    ModelState.AddModelError(string.Empty, notification);
                }
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: ProvedorController/Edit/5
        public async Task<ActionResult> Edit(Guid id)
        {
            var result = await _provedorService.GetByIdAsync(id);

            if (!result.Success)
            {
                foreach (var notification in result.Notifications)
                {
                    ModelState.AddModelError(string.Empty, notification);
                }
                return RedirectToAction(nameof(Index));
            }

            return View(result.Data);
        }

        // POST: ProvedorController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Guid id, ProvedorViewModel model)
        {
            var result = await _provedorService.UpdateAsync(id, model);

            if (!result.Success)
            {
                foreach (var notification in result.Notifications)
                {
                    ModelState.AddModelError(string.Empty, notification);
                }
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: ProvedorController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            var result = await _provedorService.DeleteAsync(id);

            // Se for requisição AJAX, retornar JSON
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
                Request.Headers.Accept.Contains("application/json"))
            {
                if (result.Success)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, errors = result.Notifications });
                }
            }

            // Se não for AJAX, comportamento padrão
            if (!result.Success)
            {
                foreach (var notification in result.Notifications)
                {
                    ModelState.AddModelError(string.Empty, notification);
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

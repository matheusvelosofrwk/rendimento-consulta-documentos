using ConsultaDocumentos.Web.Clients;
using ConsultaDocumentos.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ConsultaDocumentos.Web.Controllers
{
    public class ProvedorController : Controller
    {
        private readonly IProvedorApi _api;

        public ProvedorController(IProvedorApi api)
        {
            _api = api;
        }

        // GET: ProvedorController
        public async Task<ActionResult> Index()
        {
            var result = await _api.GetAllAsync();

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
            var result = await _api.CreateAsync(model);

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
            var result = await _api.GetByIdAsync(id);

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
            var result = await _api.UpdateAsync(id, model);

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
            var result = await _api.DeleteAsync(id);

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

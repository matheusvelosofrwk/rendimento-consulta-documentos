using ConsultaDocumentos.Web.Models;
using ConsultaDocumentos.Web.Services.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ConsultaDocumentos.Web.Controllers
{
    public class NacionalidadeController : Controller
    {
        private readonly NacionalidadeHttpService _nacionalidadeService;

        public NacionalidadeController(NacionalidadeHttpService nacionalidadeService)
        {
            _nacionalidadeService = nacionalidadeService;
        }

        // GET: NacionalidadeController
        public async Task<ActionResult> Index()
        {
            var result = await _nacionalidadeService.GetAllAsync();

            if (!result.Success)
            {
                foreach (var notification in result.Notifications)
                {
                    ModelState.AddModelError(string.Empty, notification);
                }
                return View(new List<NacionalidadeViewModel>());
            }

            return View(result.Data);
        }

        // GET: NacionalidadeController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: NacionalidadeController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync(NacionalidadeViewModel model)
        {
            var result = await _nacionalidadeService.CreateAsync(model);

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

        // GET: NacionalidadeController/Edit/5
        public async Task<ActionResult> Edit(Guid id)
        {
            var result = await _nacionalidadeService.GetByIdAsync(id);

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

        // POST: NacionalidadeController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Guid id, NacionalidadeViewModel model)
        {
            var result = await _nacionalidadeService.UpdateAsync(id, model);

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

        // POST: NacionalidadeController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            var result = await _nacionalidadeService.DeleteAsync(id);

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

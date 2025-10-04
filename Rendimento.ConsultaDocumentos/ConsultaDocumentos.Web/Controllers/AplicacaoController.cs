using ConsultaDocumentos.Web.Clients;
using ConsultaDocumentos.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ConsultaDocumentos.Web.Controllers
{
    public class AplicacaoController : Controller
    {
        private readonly IAplicacaoApi _api;

        public AplicacaoController(IAplicacaoApi api)
        {
            _api = api;
        }

        // GET: AplicacaoController
        public async Task<ActionResult> Index()
        {
            var result = await _api.GetAllAsync();

            if (!result.Success)
            {
                foreach (var notification in result.Notifications)
                {
                    ModelState.AddModelError(string.Empty, notification);
                }
                return View(new List<AplicacaoViewModel>());
            }

            return View(result.Data);
        }

        // GET: AplicacaoController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AplicacaoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync(AplicacaoViewModel model)
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

        // GET: AplicacaoController/Edit/5
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

        // POST: AplicacaoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Guid id, AplicacaoViewModel model)
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

        // GET: AplicacaoController/Delete/5
        public async Task<ActionResult> Delete(Guid id)
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

        // POST: AplicacaoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            var result = await _api.DeleteAsync(id);

            if (!result.Success)
            {
                foreach (var notification in result.Notifications)
                {
                    ModelState.AddModelError(string.Empty, notification);
                }
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

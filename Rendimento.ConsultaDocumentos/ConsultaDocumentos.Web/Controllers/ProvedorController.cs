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
            var list = await _api.GetAllAsync();

            return View(list);
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
            try
            {

                await _api.CreateAsync(model);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ProvedorController/Edit/5
        public async Task<ActionResult> Edit(Guid id)
        {
            var entity = await _api.GetByIdAsync(id);

            return View(entity);
        }

        // POST: ProvedorController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Guid id, ProvedorViewModel model)
        {
            try
            {
                await _api.UpdateAsync(id, model);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        // GET: ProvedorController/Delete/5
        public async Task<ActionResult> Delete(Guid id)
        {
            var entity = await _api.GetByIdAsync(id);

            return View(entity);
        }

        // POST: ProvedorController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                await _api.DeleteAsync(id);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}

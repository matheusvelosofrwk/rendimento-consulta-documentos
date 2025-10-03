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
            var list = await _api.GetAllAsync();

            return View(list);
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

        // GET: AplicacaoController/Edit/5
        public async Task<ActionResult> Edit(Guid id)
        {
            var entity = await _api.GetByIdAsync(id);

            return View(entity);
        }

        // POST: AplicacaoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Guid id, AplicacaoViewModel model)
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

        // GET: AplicacaoController/Delete/5
        public async Task<ActionResult> Delete(Guid id)
        {
            var entity = await _api.GetByIdAsync(id);

            return View(entity);
        }

        // POST: AplicacaoController/Delete/5
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

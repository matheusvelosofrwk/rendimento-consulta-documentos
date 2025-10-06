using ConsultaDocumentos.Web.Clients;
using ConsultaDocumentos.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ConsultaDocumentos.Web.Controllers
{
    public class QuadroSocietarioController : Controller
    {
        private readonly IQuadroSocietarioApi _api;
        private readonly INacionalidadeApi _nacionalidadeApi;

        public QuadroSocietarioController(IQuadroSocietarioApi api, INacionalidadeApi nacionalidadeApi)
        {
            _api = api;
            _nacionalidadeApi = nacionalidadeApi;
        }

        // GET: QuadroSocietarioController
        public async Task<ActionResult> Index()
        {
            var result = await _api.GetAllAsync();

            if (!result.Success)
            {
                foreach (var notification in result.Notifications)
                {
                    ModelState.AddModelError(string.Empty, notification);
                }
                return View(new List<QuadroSocietarioViewModel>());
            }

            return View(result.Data);
        }

        // GET: QuadroSocietarioController/Create
        public async Task<ActionResult> Create()
        {
            await CarregarNacionalidades();
            return View();
        }

        // POST: QuadroSocietarioController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync(QuadroSocietarioViewModel model)
        {
            var result = await _api.CreateAsync(model);

            if (!result.Success)
            {
                foreach (var notification in result.Notifications)
                {
                    ModelState.AddModelError(string.Empty, notification);
                }
                await CarregarNacionalidades();
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: QuadroSocietarioController/Edit/5
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

            await CarregarNacionalidades(result.Data.IdNacionalidade);
            return View(result.Data);
        }

        // POST: QuadroSocietarioController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Guid id, QuadroSocietarioViewModel model)
        {
            var result = await _api.UpdateAsync(id, model);

            if (!result.Success)
            {
                foreach (var notification in result.Notifications)
                {
                    ModelState.AddModelError(string.Empty, notification);
                }
                await CarregarNacionalidades(model.IdNacionalidade);
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: QuadroSocietarioController/Delete/5
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

        private async Task CarregarNacionalidades(Guid? nacionalidadeSelecionada = null)
        {
            var nacionalidades = await _nacionalidadeApi.GetAllAsync();

            if (nacionalidades.Success && nacionalidades.Data != null)
            {
                ViewBag.Nacionalidades = nacionalidades.Data.Select(n => new SelectListItem
                {
                    Value = n.Id.ToString(),
                    Text = n.Descricao,
                    Selected = n.Id == nacionalidadeSelecionada
                }).ToList();
            }
            else
            {
                ViewBag.Nacionalidades = new List<SelectListItem>();
            }
        }
    }
}

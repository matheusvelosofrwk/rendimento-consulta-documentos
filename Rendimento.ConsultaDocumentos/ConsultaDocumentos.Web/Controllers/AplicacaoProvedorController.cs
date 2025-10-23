using ConsultaDocumentos.Web.Clients;
using ConsultaDocumentos.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ConsultaDocumentos.Web.Controllers
{
    public class AplicacaoProvedorController : Controller
    {
        private readonly IAplicacaoProvedorApi _api;
        private readonly IAplicacaoApi _aplicacaoApi;
        private readonly IProvedorApi _provedorApi;

        public AplicacaoProvedorController(
            IAplicacaoProvedorApi api,
            IAplicacaoApi aplicacaoApi,
            IProvedorApi provedorApi)
        {
            _api = api;
            _aplicacaoApi = aplicacaoApi;
            _provedorApi = provedorApi;
        }

        // GET: AplicacaoProvedorController
        public async Task<ActionResult> Index(Guid? aplicacaoId)
        {
            Result<IList<AplicacaoProvedorViewModel>> result;

            if (aplicacaoId.HasValue && aplicacaoId.Value != Guid.Empty)
            {
                result = await _api.GetByAplicacaoIdAsync(aplicacaoId.Value);
            }
            else
            {
                result = await _api.GetAllAsync();
            }

            if (!result.Success)
            {
                foreach (var notification in result.Notifications)
                {
                    ModelState.AddModelError(string.Empty, notification);
                }
                return View(new List<AplicacaoProvedorViewModel>());
            }

            // Carregar lista de aplicações para o filtro
            var aplicacoesResult = await _aplicacaoApi.GetAllAsync();
            ViewBag.Aplicacoes = aplicacoesResult.Success ? aplicacoesResult.Data : new List<AplicacaoViewModel>();
            ViewBag.AplicacaoIdSelecionada = aplicacaoId;

            return View(result.Data);
        }

        // GET: AplicacaoProvedorController/Create
        public async Task<ActionResult> Create()
        {
            await LoadSelectLists();
            return View();
        }

        // POST: AplicacaoProvedorController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync(AplicacaoProvedorViewModel model)
        {
            var result = await _api.CreateAsync(model);

            if (!result.Success)
            {
                foreach (var notification in result.Notifications)
                {
                    ModelState.AddModelError(string.Empty, notification);
                }
                await LoadSelectLists();
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: AplicacaoProvedorController/Edit/5
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

            await LoadSelectLists();
            return View(result.Data);
        }

        // POST: AplicacaoProvedorController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Guid id, AplicacaoProvedorViewModel model)
        {
            var result = await _api.UpdateAsync(id, model);

            if (!result.Success)
            {
                foreach (var notification in result.Notifications)
                {
                    ModelState.AddModelError(string.Empty, notification);
                }
                await LoadSelectLists();
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: AplicacaoProvedorController/Delete/5
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

        private async Task LoadSelectLists()
        {
            var aplicacoesResult = await _aplicacaoApi.GetAllAsync();
            var provedoresResult = await _provedorApi.GetAllAsync();

            ViewBag.Aplicacoes = aplicacoesResult.Success ? aplicacoesResult.Data : new List<AplicacaoViewModel>();
            ViewBag.Provedores = provedoresResult.Success ? provedoresResult.Data : new List<ProvedorViewModel>();
        }
    }
}

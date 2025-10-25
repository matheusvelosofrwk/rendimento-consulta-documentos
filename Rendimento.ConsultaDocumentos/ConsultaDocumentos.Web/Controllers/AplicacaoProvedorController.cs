using ConsultaDocumentos.Web.Services.Http;
using ConsultaDocumentos.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ConsultaDocumentos.Web.Controllers
{
    public class AplicacaoProvedorController : Controller
    {
        private readonly AplicacaoProvedorHttpService _aplicacaoProvedorService;
        private readonly AplicacaoHttpService _aplicacaoService;
        private readonly ProvedorHttpService _provedorService;

        public AplicacaoProvedorController(
            AplicacaoProvedorHttpService aplicacaoProvedorService,
            AplicacaoHttpService aplicacaoService,
            ProvedorHttpService provedorService)
        {
            _aplicacaoProvedorService = aplicacaoProvedorService;
            _aplicacaoService = aplicacaoService;
            _provedorService = provedorService;
        }

        // GET: AplicacaoProvedorController
        public async Task<ActionResult> Index(Guid? aplicacaoId)
        {
            Result<IList<AplicacaoProvedorViewModel>> result;

            if (aplicacaoId.HasValue && aplicacaoId.Value != Guid.Empty)
            {
                result = await _aplicacaoProvedorService.GetByAplicacaoIdAsync(aplicacaoId.Value);
            }
            else
            {
                result = await _aplicacaoProvedorService.GetAllAsync();
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
            var aplicacoesResult = await _aplicacaoService.GetAllAsync();
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
            var result = await _aplicacaoProvedorService.CreateAsync(model);

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
            var result = await _aplicacaoProvedorService.GetByIdAsync(id);

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
            var result = await _aplicacaoProvedorService.UpdateAsync(id, model);

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
            var result = await _aplicacaoProvedorService.DeleteAsync(id);

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
            var aplicacoesResult = await _aplicacaoService.GetAllAsync();
            var provedoresResult = await _provedorService.GetAllAsync();

            ViewBag.Aplicacoes = aplicacoesResult.Success ? aplicacoesResult.Data : new List<AplicacaoViewModel>();
            ViewBag.Provedores = provedoresResult.Success ? provedoresResult.Data : new List<ProvedorViewModel>();
        }
    }
}

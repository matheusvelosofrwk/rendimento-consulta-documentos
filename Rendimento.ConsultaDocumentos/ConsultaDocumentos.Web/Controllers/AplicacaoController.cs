using ConsultaDocumentos.Web.Models;
using ConsultaDocumentos.Web.Services.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ConsultaDocumentos.Web.Controllers
{
    public class AplicacaoController : Controller
    {
        private readonly AplicacaoHttpService _aplicacaoService;
        private readonly AplicacaoProvedorHttpService _aplicacaoProvedorService;
        private readonly ProvedorHttpService _provedorService;

        public AplicacaoController(
            AplicacaoHttpService aplicacaoService,
            AplicacaoProvedorHttpService aplicacaoProvedorService,
            ProvedorHttpService provedorService)
        {
            _aplicacaoService = aplicacaoService;
            _aplicacaoProvedorService = aplicacaoProvedorService;
            _provedorService = provedorService;
        }

        // GET: AplicacaoController
        public async Task<ActionResult> Index()
        {
            var result = await _aplicacaoService.GetAllAsync();

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
            var result = await _aplicacaoService.CreateAsync(model);

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
            var result = await _aplicacaoService.GetByIdAsync(id);

            if (!result.Success)
            {
                foreach (var notification in result.Notifications)
                {
                    ModelState.AddModelError(string.Empty, notification);
                }
                return RedirectToAction(nameof(Index));
            }

            // Carregar provedores vinculados
            var provedoresVinculadosResult = await _aplicacaoProvedorService.GetByAplicacaoIdAsync(id);
            var provedoresDisponiveisResult = await _provedorService.GetAllAsync();

            var viewModel = new AplicacaoComProvedoresViewModel
            {
                Id = result.Data.Id,
                Nome = result.Data.Nome,
                Descricao = result.Data.Descricao,
                Status = result.Data.Status,
                Serpro = result.Data.Serpro,
                ProvedoresVinculados = provedoresVinculadosResult.Success ? provedoresVinculadosResult.Data.ToList() : new List<AplicacaoProvedorViewModel>(),
                ProvedoresDisponiveis = provedoresDisponiveisResult.Success ? provedoresDisponiveisResult.Data.ToList() : new List<ProvedorViewModel>()
            };

            return View(viewModel);
        }

        // POST: AplicacaoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Guid id, AplicacaoComProvedoresViewModel viewModel)
        {
            // Criar o modelo de aplicação para atualização
            var aplicacaoModel = new AplicacaoViewModel
            {
                Id = viewModel.Id,
                Nome = viewModel.Nome,
                Descricao = viewModel.Descricao,
                Status = viewModel.Status,
                Serpro = viewModel.Serpro
            };

            var result = await _aplicacaoService.UpdateAsync(id, aplicacaoModel);

            if (!result.Success)
            {
                foreach (var notification in result.Notifications)
                {
                    ModelState.AddModelError(string.Empty, notification);
                }

                // Recarregar dados para exibir a view novamente
                var provedoresVinculadosResult = await _aplicacaoProvedorService.GetByAplicacaoIdAsync(id);
                var provedoresDisponiveisResult = await _provedorService.GetAllAsync();

                viewModel.ProvedoresVinculados = provedoresVinculadosResult.Success ? provedoresVinculadosResult.Data.ToList() : new List<AplicacaoProvedorViewModel>();
                viewModel.ProvedoresDisponiveis = provedoresDisponiveisResult.Success ? provedoresDisponiveisResult.Data.ToList() : new List<ProvedorViewModel>();

                return View(viewModel);
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: AplicacaoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            var result = await _aplicacaoService.DeleteAsync(id);

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

        // AJAX: Adicionar provedor à aplicação
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> AdicionarProvedor([FromBody] AplicacaoProvedorViewModel model)
        {
            var result = await _aplicacaoProvedorService.CreateAsync(model);

            if (result.Success)
            {
                return Json(new { success = true, data = result.Data });
            }
            else
            {
                return Json(new { success = false, errors = result.Notifications });
            }
        }

        // AJAX: Atualizar provedor da aplicação
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> AtualizarProvedor(Guid id, [FromBody] AplicacaoProvedorViewModel model)
        {
            var result = await _aplicacaoProvedorService.UpdateAsync(id, model);

            if (result.Success)
            {
                return Json(new { success = true, data = result.Data });
            }
            else
            {
                return Json(new { success = false, errors = result.Notifications });
            }
        }

        // AJAX: Remover provedor da aplicação
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> RemoverProvedor(Guid id)
        {
            var result = await _aplicacaoProvedorService.DeleteAsync(id);

            if (result.Success)
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, errors = result.Notifications });
            }
        }
    }
}

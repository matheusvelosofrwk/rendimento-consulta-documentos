using ConsultaDocumentos.Web.Services.Http;
using ConsultaDocumentos.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace ConsultaDocumentos.Web.Controllers
{
    public class SituacaoCadastralController : Controller
    {
        private readonly SituacaoCadastralHttpService _situacaoCadastralService;

        public SituacaoCadastralController(SituacaoCadastralHttpService situacaoCadastralService)
        {
            _situacaoCadastralService = situacaoCadastralService;
        }

        // GET: SituacaoCadastralController
        public async Task<ActionResult> Index(string tipoPessoa = null)
        {
            Result<IList<SituacaoCadastralViewModel>> result;

            // Se filtrar por tipo de pessoa específico
            if (!string.IsNullOrEmpty(tipoPessoa) && tipoPessoa != "T")
            {
                char tipoPessoaChar = tipoPessoa[0];
                result = await _situacaoCadastralService.GetByTipoPessoaAsync(tipoPessoaChar);
            }
            else
            {
                // Buscar todas
                result = await _situacaoCadastralService.GetAllAsync();
            }

            if (!result.Success)
            {
                foreach (var notification in result.Notifications)
                {
                    ModelState.AddModelError(string.Empty, notification);
                }
                return View(new List<SituacaoCadastralViewModel>());
            }

            // Popula dropdown de filtro de tipo de pessoa
            ViewBag.TipoPessoaFiltro = new SelectList(new[]
            {
                new { Value = "T", Text = "Todos" },
                new { Value = "F", Text = "Pessoa Física" },
                new { Value = "J", Text = "Pessoa Jurídica" },
                new { Value = "A", Text = "Ambos" }
            }, "Value", "Text", tipoPessoa ?? "T");

            return View(result.Data);
        }

        // GET: SituacaoCadastralController/Create
        public ActionResult Create()
        {
            PopulateTipoPessoaDropdown();
            return View();
        }

        // POST: SituacaoCadastralController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync(SituacaoCadastralViewModel model)
        {
            var result = await _situacaoCadastralService.CreateAsync(model);

            if (!result.Success)
            {
                foreach (var notification in result.Notifications)
                {
                    ModelState.AddModelError(string.Empty, notification);
                }
                PopulateTipoPessoaDropdown(model.TipoPessoa);
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: SituacaoCadastralController/Edit/5
        public async Task<ActionResult> Edit(Guid id)
        {
            var result = await _situacaoCadastralService.GetByIdAsync(id);

            if (!result.Success)
            {
                foreach (var notification in result.Notifications)
                {
                    ModelState.AddModelError(string.Empty, notification);
                }
                return RedirectToAction(nameof(Index));
            }

            PopulateTipoPessoaDropdown(result.Data.TipoPessoa);
            return View(result.Data);
        }

        // POST: SituacaoCadastralController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Guid id, SituacaoCadastralViewModel model)
        {
            var result = await _situacaoCadastralService.UpdateAsync(id, model);

            if (!result.Success)
            {
                foreach (var notification in result.Notifications)
                {
                    ModelState.AddModelError(string.Empty, notification);
                }
                PopulateTipoPessoaDropdown(model.TipoPessoa);
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        // Helper para popular dropdown de tipo de pessoa
        private void PopulateTipoPessoaDropdown(char? selectedValue = null)
        {
            ViewBag.TipoPessoaList = new SelectList(new[]
            {
                new { Value = 'F', Text = "Pessoa Física" },
                new { Value = 'J', Text = "Pessoa Jurídica" },
                new { Value = 'A', Text = "Ambos" }
            }, "Value", "Text", selectedValue ?? 'A');
        }

        // POST: SituacaoCadastralController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            var result = await _situacaoCadastralService.DeleteAsync(id);

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

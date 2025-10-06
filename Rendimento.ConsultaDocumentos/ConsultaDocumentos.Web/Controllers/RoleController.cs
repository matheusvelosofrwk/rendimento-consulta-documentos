using ConsultaDocumentos.Web.Clients;
using ConsultaDocumentos.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConsultaDocumentos.Web.Controllers
{
    [Authorize]
    public class RoleController : Controller
    {
        private readonly IRoleApi _roleApi;

        public RoleController(IRoleApi roleApi)
        {
            _roleApi = roleApi;
        }

        // GET: Role
        public async Task<IActionResult> Index()
        {
            var result = await _roleApi.GetAllAsync();
            if (!result.Success)
            {
                TempData["ErrorMessage"] = string.Join(", ", result.Notifications);
                return View(new List<RoleViewModel>());
            }

            return View(result.Data);
        }

        // GET: Role/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Role/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _roleApi.CreateAsync(model);

                if (result.Success)
                {
                    TempData["SuccessMessage"] = $"Perfil '{model.Name}' criado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Notifications)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }

            return View(model);
        }

        // GET: Role/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var result = await _roleApi.GetByIdAsync(id);
            if (!result.Success || result.Data == null)
            {
                return NotFound();
            }

            var model = new UpdateRoleViewModel
            {
                Id = result.Data.Id,
                Name = result.Data.Name
            };

            return View(model);
        }

        // POST: Role/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UpdateRoleViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var result = await _roleApi.UpdateAsync(id, model);

                if (result.Success)
                {
                    TempData["SuccessMessage"] = $"Perfil '{model.Name}' atualizado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Notifications)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }

            return View(model);
        }

        // POST: Role/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var result = await _roleApi.DeleteAsync(id);

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
                    return Json(new { success = false, errors = result.Notifications.ToArray() });
                }
            }

            // Se não for AJAX, comportamento padrão
            if (result.Success)
            {
                TempData["SuccessMessage"] = "Perfil excluído com sucesso!";
            }
            else
            {
                foreach (var error in result.Notifications)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

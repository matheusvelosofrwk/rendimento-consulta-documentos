using ConsultaDocumentos.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConsultaDocumentos.Web.Controllers
{
    [Authorize]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        // GET: Role
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            var roleViewModels = roles.Select(r => new RoleViewModel
            {
                Id = r.Id,
                Name = r.Name ?? string.Empty,
                NormalizedName = r.NormalizedName
            }).ToList();

            return View(roleViewModels);
        }

        // GET: Role/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Role/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var roleExists = await _roleManager.RoleExistsAsync(model.Name);
                if (roleExists)
                {
                    ModelState.AddModelError(string.Empty, "Já existe um perfil com este nome.");
                    return View(model);
                }

                var role = new IdentityRole(model.Name);
                var result = await _roleManager.CreateAsync(role);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = $"Perfil '{model.Name}' criado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
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

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            var model = new RoleViewModel
            {
                Id = role.Id,
                Name = role.Name ?? string.Empty,
                NormalizedName = role.NormalizedName
            };

            return View(model);
        }

        // POST: Role/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, RoleViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByIdAsync(id);
                if (role == null)
                {
                    return NotFound();
                }

                // Verificar se o novo nome já existe (exceto para o próprio perfil)
                var existingRole = await _roleManager.FindByNameAsync(model.Name);
                if (existingRole != null && existingRole.Id != id)
                {
                    ModelState.AddModelError(string.Empty, "Já existe um perfil com este nome.");
                    return View(model);
                }

                role.Name = model.Name;
                var result = await _roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = $"Perfil '{model.Name}' atualizado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        // POST: Role/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
                    Request.Headers.Accept.Contains("application/json"))
                {
                    return Json(new { success = false, errors = new[] { "Perfil não encontrado." } });
                }
                return NotFound();
            }

            var result = await _roleManager.DeleteAsync(role);

            // Se for requisição AJAX, retornar JSON
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
                Request.Headers.Accept.Contains("application/json"))
            {
                if (result.Succeeded)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, errors = result.Errors.Select(e => e.Description).ToArray() });
                }
            }

            // Se não for AJAX, comportamento padrão
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"Perfil '{role.Name}' excluído com sucesso!";
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

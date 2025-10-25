using ConsultaDocumentos.Web.Services.Http;
using ConsultaDocumentos.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConsultaDocumentos.Web.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly UserHttpService _userService;
        private readonly RoleHttpService _roleService;

        public UserController(UserHttpService userService, RoleHttpService roleService)
        {
            _userService = userService;
            _roleService = roleService;
        }

        // GET: User
        public async Task<IActionResult> Index()
        {
            var result = await _userService.GetAllAsync();
            if (!result.Success)
            {
                TempData["ErrorMessage"] = string.Join(", ", result.Notifications);
                return View(new List<UserViewModel>());
            }

            return View(result.Data);
        }

        // GET: User/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.CreateAsync(model);

                if (result.Success)
                {
                    TempData["SuccessMessage"] = $"Usuário '{model.Email}' criado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Notifications)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }

            return View(model);
        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var result = await _userService.GetByIdAsync(id);
            if (!result.Success || result.Data == null)
            {
                return NotFound();
            }

            var model = new UpdateUserViewModel
            {
                Id = result.Data.Id,
                Email = result.Data.Email,
                EmailConfirmed = result.Data.EmailConfirmed
            };

            return View(model);
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UpdateUserViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            // Remover validação de senha se não foi fornecida
            if (string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.Remove("Password");
            }

            if (ModelState.IsValid)
            {
                var result = await _userService.UpdateAsync(id, model);

                if (result.Success)
                {
                    TempData["SuccessMessage"] = $"Usuário '{model.Email}' atualizado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Notifications)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }

            return View(model);
        }

        // GET: User/ManageRoles/5
        public async Task<IActionResult> ManageRoles(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var userResult = await _userService.GetByIdAsync(id);
            if (!userResult.Success || userResult.Data == null)
            {
                return NotFound();
            }

            var rolesResult = await _roleService.GetAllAsync();
            if (!rolesResult.Success)
            {
                TempData["ErrorMessage"] = "Erro ao carregar perfis";
                return RedirectToAction(nameof(Index));
            }

            var model = new UserRoleViewModel
            {
                UserId = userResult.Data.Id,
                UserEmail = userResult.Data.Email,
                Roles = rolesResult.Data.Select(r => new RoleSelectionViewModel
                {
                    RoleName = r.Name,
                    IsSelected = userResult.Data.Roles?.Contains(r.Name) ?? false
                }).ToList()
            };

            return View(model);
        }

        // POST: User/ManageRoles
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageRoles(UserRoleViewModel model)
        {
            var selectedRoles = model.Roles.Where(r => r.IsSelected).Select(r => r.RoleName).ToList();

            var manageRolesDto = new ManageUserRolesViewModel
            {
                UserId = model.UserId,
                RoleNames = selectedRoles
            };

            var result = await _userService.ManageRolesAsync(model.UserId, manageRolesDto);

            if (result.Success)
            {
                TempData["SuccessMessage"] = $"Perfis do usuário '{model.UserEmail}' atualizados com sucesso!";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Notifications)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return View(model);
        }

        // POST: User/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var result = await _userService.DeleteAsync(id);

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
                TempData["SuccessMessage"] = "Usuário excluído com sucesso!";
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

using ConsultaDocumentos.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConsultaDocumentos.Web.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: User
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userViewModels = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userViewModels.Add(new UserViewModel
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    UserName = user.UserName,
                    EmailConfirmed = user.EmailConfirmed,
                    LockoutEnabled = user.LockoutEnabled,
                    Roles = roles.ToList()
                });
            }

            return View(userViewModels);
        }

        // GET: User/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userExists = await _userManager.FindByEmailAsync(model.Email);
                if (userExists != null)
                {
                    ModelState.AddModelError(string.Empty, "Já existe um usuário com este email.");
                    return View(model);
                }

                var user = new IdentityUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, model.Password!);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = $"Usuário '{model.Email}' criado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
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

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var model = new UserViewModel
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                UserName = user.UserName,
                EmailConfirmed = user.EmailConfirmed,
                LockoutEnabled = user.LockoutEnabled,
                Roles = roles.ToList()
            };

            return View(model);
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UserViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Remover validação de senha se não foi fornecida
            if (string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.Remove("Password");
                ModelState.Remove("ConfirmPassword");
            }

            if (ModelState.IsValid)
            {
                // Verificar se o novo email já existe (exceto para o próprio usuário)
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null && existingUser.Id != id)
                {
                    ModelState.AddModelError(string.Empty, "Já existe um usuário com este email.");
                    return View(model);
                }

                user.Email = model.Email;
                user.UserName = model.Email;
                user.EmailConfirmed = model.EmailConfirmed;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    // Atualizar senha se foi fornecida
                    if (!string.IsNullOrWhiteSpace(model.Password))
                    {
                        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                        var resetResult = await _userManager.ResetPasswordAsync(user, token, model.Password);

                        if (!resetResult.Succeeded)
                        {
                            foreach (var error in resetResult.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                            return View(model);
                        }
                    }

                    TempData["SuccessMessage"] = $"Usuário '{model.Email}' atualizado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
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

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var allRoles = await _roleManager.Roles.ToListAsync();
            var userRoles = await _userManager.GetRolesAsync(user);

            var model = new UserRoleViewModel
            {
                UserId = user.Id,
                UserEmail = user.Email ?? string.Empty,
                Roles = allRoles.Select(r => new RoleSelectionViewModel
                {
                    RoleName = r.Name ?? string.Empty,
                    IsSelected = userRoles.Contains(r.Name!)
                }).ToList()
            };

            return View(model);
        }

        // POST: User/ManageRoles
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageRoles(UserRoleViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            // Remover todas as roles atuais
            var removeResult = await _userManager.RemoveFromRolesAsync(user, userRoles);
            if (!removeResult.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Erro ao remover perfis anteriores.");
                return View(model);
            }

            // Adicionar roles selecionadas
            var selectedRoles = model.Roles.Where(r => r.IsSelected).Select(r => r.RoleName).ToList();
            if (selectedRoles.Any())
            {
                var addResult = await _userManager.AddToRolesAsync(user, selectedRoles);
                if (!addResult.Succeeded)
                {
                    foreach (var error in addResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }
            }

            TempData["SuccessMessage"] = $"Perfis do usuário '{user.Email}' atualizados com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        // POST: User/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
                    Request.Headers.Accept.Contains("application/json"))
                {
                    return Json(new { success = false, errors = new[] { "Usuário não encontrado." } });
                }
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);

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
                TempData["SuccessMessage"] = $"Usuário '{user.Email}' excluído com sucesso!";
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

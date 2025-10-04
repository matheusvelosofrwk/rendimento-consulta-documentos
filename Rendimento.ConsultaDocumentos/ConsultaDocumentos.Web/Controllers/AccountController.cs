using ConsultaDocumentos.Web.Clients;
using ConsultaDocumentos.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Refit;
using System.Security.Claims;

namespace ConsultaDocumentos.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthApi _authApi;

        public AccountController(IAuthApi authApi)
        {
            _authApi = authApi;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var response = await _authApi.LoginAsync(model);

                // Criar claims do usuário
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, response.Email),
                    new Claim(ClaimTypes.Email, response.Email),
                    new Claim("access_token", response.Token)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = response.Expiration
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Home");
            }
            catch (ApiException ex)
            {
                ModelState.AddModelError(string.Empty, "Email ou senha inválidos");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Erro ao fazer login. Tente novamente.");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var response = await _authApi.RegisterAsync(model);

                // Criar claims do usuário
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, response.Email),
                    new Claim(ClaimTypes.Email, response.Email),
                    new Claim("access_token", response.Token)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = response.Expiration
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToAction("Index", "Home");
            }
            catch (ApiException ex)
            {
                ModelState.AddModelError(string.Empty, "Erro ao registrar usuário. Verifique os dados e tente novamente.");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Erro ao registrar. Tente novamente.");
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}

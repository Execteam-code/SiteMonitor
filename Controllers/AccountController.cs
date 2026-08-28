using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ServiceMonitor.Web.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;

        public AccountController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = "/")
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return LocalRedirect(returnUrl);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string password, string returnUrl = "/")
        {
            ViewData["ReturnUrl"] = returnUrl;

            var correctPassword = _configuration["DashboardPassword"];

            if (string.IsNullOrEmpty(password) || password != correctPassword)
            {
                ModelState.AddModelError(string.Empty, "Неверный пароль");
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "Admin")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, 
                new ClaimsPrincipal(claimsIdentity), 
                authProperties);

            if (Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }
            
            return RedirectToAction("Index", "Dashboard");
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}

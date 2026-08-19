using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sonara.WebUI.Models;
using Sonara.WebUI.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Sonara.WebUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly SonaraApiClient _apiClient;

        public AccountController(SonaraApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            ViewBag.Email = email;
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var deviceIdentifier = GetOrCreateDeviceIdentifier();

            var result = await _apiClient.LoginAsync(model.Email, model.Password, deviceIdentifier);

            if (result is null || !result.Success)
            {
                ModelState.AddModelError("", result?.ErrorMessage ?? "Giriş başarısız. Bilgilerini kontrol et.");
                return View(model);
            }

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(result.Token);
            var isAdmin = jwtToken.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == "Admin");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, model.Email),
                new Claim("JwtToken", result.Token!)
            };

            if (isAdmin)
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties { IsPersistent = model.RememberMe };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            if (isAdmin)
                return RedirectToAction("Index", "Admin");

            return RedirectToAction("Index", "Account");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }
        [HttpPost]
        public async Task<IActionResult>Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _apiClient.RegisterAsync(model.FirstName, model.LastName, model.Email, model.Password);

            if (result is null || !result.Success)
            {
                ModelState.AddModelError("", result?.ErrorMessage ?? "Kayıt başarısız. Bilgilerini kontrol et.");
                return View(model);
            }

      
            TempData["RegisterSuccess"] = "Hesabın oluşturuldu, şimdi giriş yapabilirsin.";
            return RedirectToAction("Login");

        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        private string GetOrCreateDeviceIdentifier()
        {
            const string cookieName = "sonara_device_id";

            if (Request.Cookies.TryGetValue(cookieName, out var existing))
                return existing;

            var newId = Guid.NewGuid().ToString();
            Response.Cookies.Append(cookieName, newId, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(2),
                HttpOnly = true
            });

            return newId;
        }
    }
}
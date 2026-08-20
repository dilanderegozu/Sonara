using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sonara.WebUI.Services;
using System.Security.Claims;

namespace Sonara.WebUI.Controllers
{
    [Authorize]
    public class PackagesController : Controller
    {
        private readonly SonaraApiClient _apiClient;

        public PackagesController(SonaraApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IActionResult> Index()
        {
            var jwtToken = User.FindFirstValue("JwtToken");
            if (jwtToken is null) return RedirectToAction("Login", "Account");

            var plans = await _apiClient.GetPlansAsync(jwtToken);
            var membership = await _apiClient.GetMyMembershipAsync(jwtToken);

            ViewBag.CurrentPlanName = membership?.PlanName ?? "Free";
            ViewBag.CurrentLevel = membership?.Level ?? 0;

            return View((plans ?? new List<PlanDto>()).OrderBy(p => p.Level).ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upgrade(int planId)
        {
            var jwtToken = User.FindFirstValue("JwtToken");
            if (jwtToken is null) return RedirectToAction("Login", "Account");

            var (success, error) = await _apiClient.PurchasePlanAsync(jwtToken, planId);

            TempData["Message"] = success ? "Paketin başarıyla güncellendi." : $"Sorun: {error}";
            return RedirectToAction("Index");
        }
    }
}
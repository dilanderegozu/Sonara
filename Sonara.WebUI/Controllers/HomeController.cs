using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sonara.WebUI.Models;
using Sonara.WebUI.Services;
using System.Security.Claims;

namespace Sonara.WebUI.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly SonaraApiClient _apiClient;

        public HomeController(SonaraApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IActionResult> Index()
        {
            var jwtToken = User.FindFirstValue("JwtToken");

            if (jwtToken is null)
                return RedirectToAction("Login", "Account");

            var membership = await _apiClient.GetMyMembershipAsync(jwtToken);
            var recentSongs = await _apiClient.GetRecentlyAddedAsync(jwtToken);
            var popularArtists = await _apiClient.GetPopularArtistsAsync(jwtToken);

            var model = new HomeViewModel
            {
                FirstName = User.FindFirstValue(ClaimTypes.GivenName) ?? "Dinleyici",
                Greeting = GetTimeBasedGreeting(),
                PlanName = membership?.PlanName ?? "Free",
                PlanLevel = membership?.Level ?? 0,
                MaxPlanLevel = (membership?.MaxLevel ?? 0) > 0 ? membership!.MaxLevel : 1,
                RecentlyAdded = recentSongs ?? new List<RecentSongDto>(),
                PopularArtists = popularArtists ?? new List<PopularArtistDto>()
            };

            return View(model);
        }

        private static string GetTimeBasedGreeting()
        {
            var hour = DateTime.Now.Hour;

            return hour switch
            {
                >= 5 and < 12 => "Günaydın",
                >= 12 and < 17 => "İyi günler",
                >= 17 and < 22 => "İyi akşamlar",
                _ => "İyi geceler"
            };
        }
    }
}
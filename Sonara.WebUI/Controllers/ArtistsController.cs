using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sonara.WebUI.Services;
using System.Security.Claims;

namespace Sonara.WebUI.Controllers
{
    [Authorize]
    public class ArtistsController : Controller
    {
        private readonly SonaraApiClient _apiClient;

        public ArtistsController(SonaraApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IActionResult> Index()
        {
            var jwtToken = User.FindFirstValue("JwtToken");
            if (jwtToken is null) return RedirectToAction("Login", "Account");

            var artists = await _apiClient.GetAllArtistsAsync(jwtToken);
            return View(artists ?? new List<PopularArtistDto>());
        }
    }
}
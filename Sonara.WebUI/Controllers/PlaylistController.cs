using Microsoft.AspNetCore.Mvc;
using Sonara.WebUI.Services;
using System.Security.Claims;

namespace Sonara.WebUI.Controllers
{
    public class PlaylistController : Controller
    {
        private readonly SonaraApiClient _apiClient;

        public PlaylistController(SonaraApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePlaylist(string name)
        {
            var jwtToken = User.FindFirstValue("JwtToken");
            if (jwtToken is null) return RedirectToAction("Login", "Account");

            await _apiClient.CreatePlaylistAsync(jwtToken, name, null);

            return RedirectToAction("Index", "Home");
        }
    }
}

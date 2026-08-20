using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sonara.WebUI.Services;
using System.Security.Claims;

namespace Sonara.WebUI.Controllers
{
    [Authorize]
    public class SongsController : Controller
    {
        private readonly SonaraApiClient _apiClient;

        public SongsController(SonaraApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IActionResult> Index()
        {
            var jwtToken = User.FindFirstValue("JwtToken");
            if (jwtToken is null) return RedirectToAction("Login", "Account");

            var songs = await _apiClient.GetAllSongsAsync(jwtToken);
            return View(songs ?? new List<AllSongDto>());
        }

        public async Task<IActionResult> Artist(int id)
        {
            var jwtToken = User.FindFirstValue("JwtToken");
            if (jwtToken is null) return RedirectToAction("Login", "Account");

            var artistSongs = await _apiClient.GetArtistSongsAsync(jwtToken, id);
            if (artistSongs is null) return NotFound();

            var allSongs = await _apiClient.GetAllSongsAsync(jwtToken);
            ViewBag.TopTracks = (allSongs ?? new List<AllSongDto>())
                .OrderByDescending(s => s.PlayCount)
                .Take(5)
                .ToList();

            return View(artistSongs);
        }
    }
}
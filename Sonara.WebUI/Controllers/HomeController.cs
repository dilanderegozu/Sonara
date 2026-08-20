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
            var playlists = await _apiClient.GetMyPlaylistsAsync(jwtToken);
            var continueListening = await _apiClient.GetContinueListeningAsync(jwtToken);
            var model = new HomeViewModel
            {
                FirstName = User.FindFirstValue(ClaimTypes.GivenName) ?? "Dinleyici",
                Greeting = GetTimeBasedGreeting(),
                PlanName = membership?.PlanName ?? "Free",
                PlanLevel = membership?.Level ?? 0,
                MaxPlanLevel = (membership?.MaxLevel ?? 0) > 0 ? membership!.MaxLevel : 1,
                RecentlyAdded = recentSongs ?? new List<RecentSongDto>(),
                PopularArtists = popularArtists ?? new List<PopularArtistDto>(),
                Playlists = playlists ?? new List<PlaylistDto>(),
                ContinueListening = continueListening ?? new List<ContinueListeningDto>()
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaySong([FromBody] PlaySongRequest req)
        {
            var jwtToken = User.FindFirstValue("JwtToken");
            if (jwtToken is null) return Unauthorized();

            var (success, data, error) = await _apiClient.PlaySongAsync(jwtToken, req.SongId);
            if (!success) return StatusCode(403, new { message = error });

            return Ok(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveProgress([FromBody] SaveProgressRequest req)
        {
            var jwtToken = User.FindFirstValue("JwtToken");
            if (jwtToken is null) return Unauthorized();

            await _apiClient.SaveProgressAsync(jwtToken, req.SongId, req.PositionSeconds);
            return Ok();
        }

        public class PlaySongRequest { public int SongId { get; set; } }
        public class SaveProgressRequest { public int SongId { get; set; } public int PositionSeconds { get; set; } }
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
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
            var moods = await _apiClient.GetMoodsAsync(jwtToken);
            var recommendations = await _apiClient.GetRecommendationsAsync(jwtToken);

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
                ContinueListening = continueListening ?? new List<ContinueListeningDto>(),
                Moods = moods ?? new List<MoodDto>(),
                Recommendations = recommendations ?? new List<RecommendedSongDto>()
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

            if (!success)
            {
                string message = "Bu şarkı şu an oynatılamıyor.";
                try
                {
                    using var doc = System.Text.Json.JsonDocument.Parse(error ?? "{}");
                    if (doc.RootElement.TryGetProperty("message", out var msgProp))
                        message = msgProp.GetString() ?? message;
                    else if (doc.RootElement.TryGetProperty("Message", out var msgPropCap))
                        message = msgPropCap.GetString() ?? message;
                }
                catch { }

                return StatusCode(403, new { message });
            }

            return Ok(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSongToPlaylist([FromBody] AddSongToPlaylistRequest req)
        {
            var jwtToken = User.FindFirstValue("JwtToken");
            if (jwtToken is null) return Unauthorized();

            var success = await _apiClient.AddSongToPlaylistAsync(jwtToken, req.PlaylistId, req.SongId);
            return Ok(new { success });
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePlaylistAjax([FromBody] CreatePlaylistRequest req)
        {
            var jwtToken = User.FindFirstValue("JwtToken");
            if (jwtToken is null) return Unauthorized();

            var (success, playlistId) = await _apiClient.CreatePlaylistWithIdAsync(jwtToken, req.Name, null);

            if (!success) return StatusCode(400, new { message = "Playlist oluşturulamadı." });

            return Ok(new { playlistId });
        }
        public async Task<IActionResult> MoodDetail(int id)
        {
            var jwtToken = User.FindFirstValue("JwtToken");
            if (jwtToken is null) return RedirectToAction("Login", "Account");

            var mood = await _apiClient.GetMoodDetailAsync(jwtToken, id);
            if (mood is null) return NotFound();

            var allMoods = await _apiClient.GetMoodsAsync(jwtToken);
            ViewBag.OtherMoods = (allMoods ?? new List<MoodDto>())
                .Where(m => m.MoodId != id)
                .ToList();

            return View(mood);
        }

        public class CreatePlaylistRequest { public string Name { get; set; } }
        public class PlaySongRequest { public int SongId { get; set; } }
        public class SaveProgressRequest { public int SongId { get; set; } public int PositionSeconds { get; set; } }
        public class AddSongToPlaylistRequest { public int PlaylistId { get; set; } public int SongId { get; set; } }
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
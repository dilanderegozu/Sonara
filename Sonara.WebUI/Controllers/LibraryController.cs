using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sonara.WebUI.Services;
using System.Security.Claims;

namespace Sonara.WebUI.Controllers
{
    [Authorize]
    public class LibraryController : Controller
    {
        private readonly SonaraApiClient _apiClient;

        public LibraryController(SonaraApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IActionResult> Index()
        {
            var jwtToken = User.FindFirstValue("JwtToken");
            if (jwtToken is null) return RedirectToAction("Login", "Account");

            var playlists = await _apiClient.GetAllPlaylistsAsync(jwtToken);
            return View(playlists ?? new List<LibraryPlaylistDto>());
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
        public async Task<IActionResult> Playlist(int id)
        {
            var jwtToken = User.FindFirstValue("JwtToken");
            if (jwtToken is null) return RedirectToAction("Login", "Account");

            var playlist = await _apiClient.GetPlaylistDetailAsync(jwtToken, id);
            if (playlist is null) return NotFound();

            return View(playlist);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveSong([FromBody] RemoveSongRequest req)
        {
            var jwtToken = User.FindFirstValue("JwtToken");
            if (jwtToken is null) return Unauthorized();

            var success = await _apiClient.RemoveSongFromPlaylistAsync(jwtToken, req.PlaylistId, req.SongId);
            return Ok(new { success });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePlaylistCover(int playlistId, IFormFile coverFile)
        {
            var jwtToken = User.FindFirstValue("JwtToken");
            if (jwtToken is null) return Unauthorized();

            using var stream = coverFile.OpenReadStream();
            var success = await _apiClient.UpdatePlaylistCoverAsync(jwtToken, playlistId, stream, coverFile.FileName, coverFile.ContentType);

            return Ok(new { success });
        }

        public class RemoveSongRequest { public int PlaylistId { get; set; } public int SongId { get; set; } }

        public class CreatePlaylistRequest { public string Name { get; set; } }
    }
}
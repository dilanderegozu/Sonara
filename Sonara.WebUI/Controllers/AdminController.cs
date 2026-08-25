using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sonara.WebUI.Services;
using System.Security.Claims;

namespace Sonara.WebUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly SonaraApiClient _apiClient;

        public AdminController(SonaraApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IActionResult> Index()
        {
            var jwtToken = User.FindFirstValue("JwtToken");
            if (jwtToken is null) return RedirectToAction("Login", "Account");

            var stats = await _apiClient.GetAdminStatsAsync(jwtToken);
            return View(stats ?? new AdminStatsDto());
        }

        public async Task<IActionResult> Songs()
        {
            var jwtToken = User.FindFirstValue("JwtToken");
            if (jwtToken is null) return RedirectToAction("Login", "Account");

            var songs = await _apiClient.GetAllSongsAsync(jwtToken);
            var artists = await _apiClient.GetAllArtistsAsync(jwtToken);
            ViewBag.Artists = artists ?? new List<PopularArtistDto>();
            return View(songs ?? new List<AllSongDto>());
        }
        public async Task<IActionResult> CreateSong()
        {
            var jwtToken = User.FindFirstValue("JwtToken");
            if (jwtToken is null) return RedirectToAction("Login", "Account");

            var artists = await _apiClient.GetAllArtistsAsync(jwtToken);
            ViewBag.Artists = artists ?? new List<PopularArtistDto>();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSong(string title, int artistId, List<int> allowedPlanIds, IFormFile audioFile, IFormFile? coverFile)
        {
            var jwtToken = User.FindFirstValue("JwtToken");
            if (jwtToken is null) return RedirectToAction("Login", "Account");

            using var audioStream = audioFile.OpenReadStream();
            Stream? coverStream = coverFile?.OpenReadStream();

            var success = await _apiClient.CreateSongAdminAsync(
                jwtToken, title, artistId, allowedPlanIds,
                audioStream, audioFile.FileName, audioFile.ContentType,
                coverStream, coverFile?.FileName, coverFile?.ContentType);

            coverStream?.Dispose();

            TempData["Message"] = success ? "Şarkı eklendi." : "Şarkı eklenemedi.";
            return RedirectToAction("Songs");
        }
        public async Task<IActionResult> Artists()
        {
            var jwtToken = User.FindFirstValue("JwtToken");
            if (jwtToken is null) return RedirectToAction("Login", "Account");

            var artists = await _apiClient.GetAllArtistsAsync(jwtToken);
            return View(artists ?? new List<PopularArtistDto>());
        }

        public IActionResult CreateArtist() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateArtist(string name, string? bio, IFormFile? photoFile)
        {
            var jwtToken = User.FindFirstValue("JwtToken");
            if (jwtToken is null) return RedirectToAction("Login", "Account");

            Stream? photoStream = photoFile?.OpenReadStream();
            var success = await _apiClient.CreateArtistAdminAsync(jwtToken, name, bio, photoStream, photoFile?.FileName, photoFile?.ContentType);
            photoStream?.Dispose();

            TempData["Message"] = success ? "Sanatçı eklendi." : "Sanatçı eklenemedi.";
            return RedirectToAction("Artists");
        }

        public async Task<IActionResult> Moods()
        {
            var jwtToken = User.FindFirstValue("JwtToken");
            if (jwtToken is null) return RedirectToAction("Login", "Account");

            var moods = await _apiClient.GetMoodsAsync(jwtToken);
            return View(moods ?? new List<MoodDto>());
        }

        public IActionResult CreateMood() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMood(string name, string colorHex)
        {
            var jwtToken = User.FindFirstValue("JwtToken");
            if (jwtToken is null) return RedirectToAction("Login", "Account");

            var success = await _apiClient.CreateMoodAdminAsync(jwtToken, name, colorHex);
            TempData["Message"] = success ? "Ruh hali eklendi." : "Ruh hali eklenemedi.";
            return RedirectToAction("Moods");
        }
        public async Task<IActionResult> Plans()
        {
            var jwtToken = User.FindFirstValue("JwtToken");
            if (jwtToken is null) return RedirectToAction("Login", "Account");

            var plans = await _apiClient.GetAllPlansAdminAsync(jwtToken);
            return View(plans ?? new List<PlanDto>());
        }

        public async Task<IActionResult> EditPlan(int? id)
        {
            var jwtToken = User.FindFirstValue("JwtToken");
            if (jwtToken is null) return RedirectToAction("Login", "Account");

            if (id is null) return View(new PlanDto());

            var plan = await _apiClient.GetPlanByIdAsync(jwtToken, id.Value);
            if (plan is null) return NotFound();
            return View(plan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPlan(int? id, string name, int level, decimal price, int maxDeviceCount, bool hasAds, bool hasOfflineDownload, bool hasHighQualityAudio, int durationInDays)
        {
            var jwtToken = User.FindFirstValue("JwtToken");
            if (jwtToken is null) return RedirectToAction("Login", "Account");

            var form = new PlanFormRequest
            {
                Name = name,
                Level = level,
                Price = price,
                MaxDeviceCount = maxDeviceCount,
                HasAds = hasAds,
                HasOfflineDownload = hasOfflineDownload,
                HasHighQualityAudio = hasHighQualityAudio,
                DurationInDays = durationInDays
            };

            bool success = id is null
                ? await _apiClient.CreatePlanAsync(jwtToken, form)
                : await _apiClient.UpdatePlanAsync(jwtToken, id.Value, form);

            TempData["Message"] = success ? "Paket kaydedildi." : "Paket kaydedilemedi.";
            return RedirectToAction("Plans");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSongAjax(int id)
        {
            var jwtToken = User.FindFirstValue("JwtToken");
            if (jwtToken is null) return Unauthorized();
            var success = await _apiClient.DeleteSongAsync(jwtToken, id);
            return Ok(new { success });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSongAjax(int id, string title, int artistId, IFormFile? coverFile)
        {
            var jwtToken = User.FindFirstValue("JwtToken");
            if (jwtToken is null) return Unauthorized();

            Stream? coverStream = coverFile?.OpenReadStream();
            var success = await _apiClient.UpdateSongAsync(jwtToken, id, title, artistId, coverStream, coverFile?.FileName, coverFile?.ContentType);
            coverStream?.Dispose();

            return Ok(new { success });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteArtistAjax(int id)
        {
            var jwtToken = User.FindFirstValue("JwtToken");
            if (jwtToken is null) return Unauthorized();
            var success = await _apiClient.DeleteArtistAsync(jwtToken, id);
            return Ok(new { success });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateArtistAjax(int id, string name, string? bio, IFormFile? photoFile)
        {
            var jwtToken = User.FindFirstValue("JwtToken");
            if (jwtToken is null) return Unauthorized();

            Stream? photoStream = photoFile?.OpenReadStream();
            var success = await _apiClient.UpdateArtistAsync(jwtToken, id, name, bio, photoStream, photoFile?.FileName, photoFile?.ContentType);
            photoStream?.Dispose();

            return Ok(new { success });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMoodAjax(int id)
        {
            var jwtToken = User.FindFirstValue("JwtToken");
            if (jwtToken is null) return Unauthorized();
            var success = await _apiClient.DeleteMoodAsync(jwtToken, id);
            return Ok(new { success });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateMoodAjax(int id, string name, string colorHex)
        {
            var jwtToken = User.FindFirstValue("JwtToken");
            if (jwtToken is null) return Unauthorized();
            var success = await _apiClient.UpdateMoodAsync(jwtToken, id, name, colorHex);
            return Ok(new { success });
        }
    }
}
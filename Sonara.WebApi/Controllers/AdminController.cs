using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sonara.CoreLayer;
using Sonara.CoreLayer.Entities;
using Sonara.DataAccessLayer.Repositories.Implementations;
using Sonara.DataAccessLayer.Repositories.Interfaces;
using Sonara.DtoLayer.Dtos.Admin;
using Sonara.DtoLayer.Dtos.Song;
using System.Diagnostics;
using TagLib;

namespace Sonara.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDashboardDal _dashboardDal;
        private readonly ISongDal _songDal;
        private readonly IBlobStorageService _blobStorageService;
        private readonly IConfiguration _configuration;
        private readonly IArtistDal _artistDal;
        private readonly IMoodDal _moodDal;
        private readonly IMembershipPlanDal _membershipPlanDal;

        public AdminController(UserManager<ApplicationUser> userManager, IDashboardDal dashboardDal, ISongDal songDal, IBlobStorageService blobStorageService, IConfiguration configuration, IArtistDal artistDal, IMoodDal moodDal, IMembershipPlanDal membershipPlanDal)
        {
            _userManager = userManager;
            _dashboardDal = dashboardDal;
            _songDal = songDal;
            _blobStorageService = blobStorageService;
            _configuration = configuration;
            _artistDal = artistDal;
            _moodDal = moodDal;
            _membershipPlanDal = membershipPlanDal;
        }


        [HttpDelete("songs/{id}")]
        public async Task<IActionResult> DeleteSong(int id)
        {
            var song = await _songDal.GetByIdAsync(id);
            if (song is null) return NotFound();

            _songDal.Delete(song);
            await _songDal.SaveChangesAsync();
            return Ok();
        }
        [HttpPut("songs/{id}")]
        public async Task<IActionResult> UpdateSong(int id, [FromForm] string title, [FromForm] int artistId, [FromForm] IFormFile? coverFile)
        {
            var song = await _songDal.GetByIdAsync(id);
            if (song is null) return NotFound();

            song.Title = title;
            song.ArtistId = artistId;

            if (coverFile is not null)
            {
                var coversContainer = _configuration["AzureStorage:CoversContainer"]!;
                using var stream = coverFile.OpenReadStream();
                song.CoverImageUrl = await _blobStorageService.UploadFileAsync(stream, coverFile.FileName, coversContainer, coverFile.ContentType);
            }

            _songDal.Update(song);
            await _songDal.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("artists/{id}")]
        public async Task<IActionResult> DeleteArtist(int id)
        {
            var artist = await _artistDal.GetByIdAsync(id);
            if (artist is null) return NotFound();

            _artistDal.Delete(artist);
            await _artistDal.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("artists/{id}")]
        public async Task<IActionResult> UpdateArtist(int id, [FromForm] string name, [FromForm] string? bio, [FromForm] IFormFile? photoFile)
        {
            var artist = await _artistDal.GetByIdAsync(id);
            if (artist is null) return NotFound();

            artist.Name = name;
            artist.Bio = bio ?? artist.Bio;

            if (photoFile is not null)
            {
                var artistsContainer = _configuration["AzureStorage:ArtistsContainer"]!;
                using var stream = photoFile.OpenReadStream();
                artist.ImageUrl = await _blobStorageService.UploadFileAsync(stream, photoFile.FileName, artistsContainer, photoFile.ContentType);
            }

            _artistDal.Update(artist);
            await _artistDal.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("moods/{id}")]
        public async Task<IActionResult> DeleteMood(int id)
        {
            var mood = await _moodDal.GetByIdAsync(id);
            if (mood is null) return NotFound();

            _moodDal.Delete(mood);
            await _moodDal.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("moods/{id}")]
        public async Task<IActionResult> UpdateMood(int id, [FromBody] UpdateMoodDto dto)
        {
            var mood = await _moodDal.GetByIdAsync(id);
            if (mood is null) return NotFound();

            mood.Name = dto.Name;
            mood.ColorHex = dto.ColorHex;

            _moodDal.Update(mood);
            await _moodDal.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var songs = await _songDal.GetAllAsync();
            var artists = await _artistDal.GetAllAsync();
            var moods = await _moodDal.GetAllAsync();
            var users = await _userManager.Users.ToListAsync();
            var plans = await _membershipPlanDal.GetAllAsync();

            var today = DateTime.UtcNow.Date;

            var result = new
            {
                TotalSongs = songs.Count,
                SongsToday = songs.Count(s => s.ReleaseDate.Date == today),
                TotalArtists = artists.Count,
                ArtistsToday = artists.Count(a => a.CreatedDate.Date == today),
                TotalMoods = moods.Count,
                TotalUsers = users.Count,
                UsersToday = users.Count(u => u.RegisteredAt.Date == today),
                TotalPlans = plans.Count,
                TotalPlayCount = songs.Sum(s => s.PlayCount)
            };

            return Ok(result);
        }

        [HttpPost("moods")]
        public async Task<IActionResult> CreateMood([FromBody] CreateMoodDto dto)
        {
            var mood = new Mood { Name = dto.Name, ColorHex = dto.ColorHex };
            await _moodDal.AddAsync(mood);
            await _moodDal.SaveChangesAsync();

            return Ok(new { mood.MoodId, mood.Name, mood.ColorHex });
        }

        [HttpPost("songs/{songId}/moods")]
        public async Task<IActionResult> AssignMoodsToSong(int songId, [FromBody] List<int> moodIds)
        {
            var song = await _songDal.GetByIdAsync(songId);
            if (song is null) return NotFound(new { Message = "Şarkı bulunamadı." });

            await _songDal.AddMoodsAsync(songId, moodIds);
            return Ok(new { Message = "Mood'lar atandı." });
        }

        [HttpPost("artists")]
        public async Task<IActionResult> CreateArtist([FromForm] CreateArtistDto dto)
        {
            var artistsContainer = _configuration["AzureStorage:ArtistsContainer"]!;

            string? imageUrl = null;
            if (dto.PhotoFile is not null)
            {
                using var stream = dto.PhotoFile.OpenReadStream();
                imageUrl = await _blobStorageService.UploadFileAsync(stream, dto.PhotoFile.FileName, artistsContainer);
            }

            var artist = new Artist
            {
                Name = dto.Name,
                Bio = dto.Bio ?? "",
                ImageUrl = imageUrl ?? "",
                MonthlyListeners = 0,
                IsVerified = false,
                CreatedDate = DateTime.UtcNow
            };

            await _artistDal.AddAsync(artist);
            await _artistDal.SaveChangesAsync();

            return Ok(new { artist.ArtistId, artist.Name, artist.ImageUrl });
        }
        [HttpPut("artists/{id}/photo")]
        public async Task<IActionResult> UpdateArtistPhoto(int id, IFormFile photoFile)
        {
            var artist = await _artistDal.GetByIdAsync(id);
            if (artist is null)
                return NotFound(new { Message = "Sanatçı bulunamadı." });

            var artistsContainer = _configuration["AzureStorage:ArtistsContainer"]!;

            using (var stream = photoFile.OpenReadStream())
            {
                artist.ImageUrl = await _blobStorageService.UploadFileAsync(stream, photoFile.FileName, artistsContainer);
            }

            _artistDal.Update(artist);
            await _artistDal.SaveChangesAsync();

            return Ok(new { artist.ArtistId, artist.Name, artist.ImageUrl });
        }

        [HttpPost("songs")]
        public async Task<IActionResult> CreateSong([FromForm] CreateSongDto dto)
        {
            var songsContainer = _configuration["AzureStorage:SongsContainer"]!;
            var coversContainer = _configuration["AzureStorage:CoversContainer"]!;

            string audioUrl;
            using (var stream = dto.AudioFile.OpenReadStream())
            {
                audioUrl = await _blobStorageService.UploadFileAsync(stream, dto.AudioFile.FileName, songsContainer, dto.AudioFile.ContentType);
            }

            string? coverUrl = null;
            if (dto.CoverFile is not null)
            {
                using var stream = dto.CoverFile.OpenReadStream();
                coverUrl = await _blobStorageService.UploadFileAsync(stream, dto.CoverFile.FileName, coversContainer, dto.CoverFile.ContentType);
            }
            TimeSpan duration = TimeSpan.Zero;
            try
            {
                var tempPath = Path.GetTempFileName();
                using (var tempStream = new FileStream(tempPath, FileMode.Create))
                {
                    await dto.AudioFile.CopyToAsync(tempStream);
                }

                var tagFile = TagLib.File.Create(tempPath);
                duration = tagFile.Properties.Duration;
                tagFile.Dispose();

                System.IO.File.Delete(tempPath);
            }
            catch
            {
                duration = TimeSpan.Zero;
            }
            var song = new Song
            {
                Title = dto.Title,
                ArtistId = dto.ArtistId,
                AlbumId = dto.AlbumId,
                AudioUrl = audioUrl,
                CoverImageUrl = coverUrl ?? "",
                Duration = duration,
                PlayCount = 0,
                ReleaseDate = DateTime.UtcNow
            };

            await _songDal.AddAsync(song);
            await _songDal.SaveChangesAsync();

            foreach (var planId in dto.AllowedPlanIds)
            {
                await _songDal.AddAllowedPlansAsync(song.SongId, dto.AllowedPlanIds);
            }

            return Ok(new { song.SongId, song.AudioUrl, song.CoverImageUrl });
        }

        [HttpGet("dashboard-summary")]
        public async Task<IActionResult> GetDashboardSummary()
        {
            var summary = new DashboardSummaryDto
            {
                TodayRegistrations = await _dashboardDal.GetTodayRegistrationCountAsync(),
                TodayPurchases = await _dashboardDal.GetTodayPurchaseCountAsync(),
                TotalRevenue = await _dashboardDal.GetTotalRevenueAsync()
            };
            return Ok(summary);
        }
        
        [HttpGet("top-songs")]
        public async Task<IActionResult> GetTopSongs([FromQuery] int count = 10)
        {
            var songs = await _dashboardDal.GetTopPlayedSongsAsync(count);
            var result = songs.Select(s => new TopSongDto
            {
                SongId = s.SongId,
                Title = s.Title,
                PlayCount = s.PlayCount
            });
            return Ok(result);
        }

        [HttpGet("top-artists")]
        public async Task<IActionResult> GetTopArtists([FromQuery] int count = 10)
        {
            var artists = await _dashboardDal.GetTopArtistsAsync(count);

            var result = artists.Select(x => new TopArtistDto
            {
                ArtistId = x.Artist.ArtistId,
                Name = x.Artist.Name,
                TotalPlays = x.TotalPlays
            });

            return Ok(result);
        }

        [HttpPost("songs/backfill-durations")]
        public async Task<IActionResult> BackfillDurations()
        {
            var songs = await _songDal.GetAllAsync();
            var updated = 0;

            using var httpClient = new HttpClient();

            foreach (var song in songs.Where(s => s.Duration == TimeSpan.Zero))
            {
                try
                {
                    var bytes = await httpClient.GetByteArrayAsync(song.AudioUrl);

                    var tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.mp3");
                    await System.IO.File.WriteAllBytesAsync(tempPath, bytes);

                    var tagFile = TagLib.File.Create(tempPath);
                    song.Duration = tagFile.Properties.Duration;
                    tagFile.Dispose();

                    System.IO.File.Delete(tempPath);

                    _songDal.Update(song);
                    updated++;
                }
                catch
                {
                    
                }
            }

            await _songDal.SaveChangesAsync();

            return Ok(new { Message = $"{updated} şarkının süresi güncellendi." });
        }
        [HttpGet("plans/{id}")]
        public async Task<IActionResult> GetPlanById(int id)
        {
            var plan = await _membershipPlanDal.GetByIdAsync(id);
            if (plan is null) return NotFound();
            return Ok(plan);
        }

        [HttpPost("plans")]
        public async Task<IActionResult> CreatePlan([FromBody] PlanFormDto dto)
        {
            var plan = new MembershipPlan
            {
                Name = dto.Name,
                Level = dto.Level,
                Price = dto.Price,
                MaxDeviceCount = dto.MaxDeviceCount,
                HasAds = dto.HasAds,
                HasOfflineDownload = dto.HasOfflineDownload,
                HasHighQualityAudio = dto.HasHighQualityAudio,
                DurationInDays = dto.DurationInDays
            };

            await _membershipPlanDal.AddAsync(plan);
            await _membershipPlanDal.SaveChangesAsync();

            return Ok(new { plan.Id });
        }

        [HttpPut("plans/{id}")]
        public async Task<IActionResult> UpdatePlan(int id, [FromBody] PlanFormDto dto)
        {
            var plan = await _membershipPlanDal.GetByIdAsync(id);
            if (plan is null) return NotFound();

            plan.Name = dto.Name;
            plan.Level = dto.Level;
            plan.Price = dto.Price;
            plan.MaxDeviceCount = dto.MaxDeviceCount;
            plan.HasAds = dto.HasAds;
            plan.HasOfflineDownload = dto.HasOfflineDownload;
            plan.HasHighQualityAudio = dto.HasHighQualityAudio;
            plan.DurationInDays = dto.DurationInDays;

            _membershipPlanDal.Update(plan);
            await _membershipPlanDal.SaveChangesAsync();

            return Ok();
        }

    }
}

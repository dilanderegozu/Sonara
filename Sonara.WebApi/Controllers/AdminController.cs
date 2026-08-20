using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sonara.CoreLayer;
using Sonara.CoreLayer.Entities;
using Sonara.DataAccessLayer.Repositories.Interfaces;
using Sonara.DtoLayer.Dtos.Admin;
using System.Diagnostics;
using TagLib;

namespace Sonara.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IDashboardDal _dashboardDal;
        private readonly ISongDal _songDal;
        private readonly IBlobStorageService _blobStorageService;
        private readonly IConfiguration _configuration;
        private readonly IArtistDal _artistDal;
        private readonly IMoodDal _moodDal;

        public AdminController(IDashboardDal dashboardDal, ISongDal songDal, IBlobStorageService blobStorageService, IConfiguration configuration, IArtistDal artistDal, IMoodDal moodDal)
        {
            _dashboardDal = dashboardDal;
            _songDal = songDal;
            _blobStorageService = blobStorageService;
            _configuration = configuration;
            _artistDal = artistDal;
            _moodDal = moodDal;
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
                    // bu şarkı atlanır, diğerlerine devam edilir
                }
            }

            await _songDal.SaveChangesAsync();

            return Ok(new { Message = $"{updated} şarkının süresi güncellendi." });
        }
    }
}

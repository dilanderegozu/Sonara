using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sonara.CoreLayer;
using Sonara.CoreLayer.Entities;
using Sonara.DataAccessLayer.Repositories.Interfaces;
using Sonara.DtoLayer.Dtos.Playlist;
using System.Security.Claims;

namespace Sonara.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PlaylistController : ControllerBase
    {
        private readonly IPlaylistDal _playlistDal;
        private readonly IBlobStorageService _blobStorageService;
        private readonly IConfiguration _configuration;

        public PlaylistController(IPlaylistDal playlistDal, IBlobStorageService blobStorageService, IConfiguration configuration)
        {
            _playlistDal = playlistDal;
            _blobStorageService = blobStorageService;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePlaylist(CreatePlaylistDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null) return Unauthorized();

            var playlist = new Playlist
            {
                Name = dto.Name,
                Description = dto.Description,
                UserId = userId,
                CreatedDate = DateTime.UtcNow
            };

            await _playlistDal.AddAsync(playlist);
            await _playlistDal.SaveChangesAsync();

            return Ok(new { playlist.PlaylistId, playlist.Name });
        }

        [HttpPost("{id}/songs")]
        public async Task<IActionResult> AddSong(int id, AddSongToPlaylistDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var playlist = await _playlistDal.GetByIdAsync(id);

            if (playlist is null || playlist.UserId != userId)
                return NotFound(new { Message = "Playlist bulunamadı." });

            await _playlistDal.AddSongAsync(id, dto.SongId);
            return Ok(new { Message = "Şarkı eklendi." });
        }

        [HttpDelete("{id}/songs/{songId}")]
        public async Task<IActionResult> RemoveSong(int id, int songId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var playlist = await _playlistDal.GetByIdAsync(id);

            if (playlist is null || playlist.UserId != userId)
                return NotFound(new { Message = "Playlist bulunamadı." });

            await _playlistDal.RemoveSongAsync(id, songId);
            return Ok(new { Message = "Şarkı çıkarıldı." });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlaylist(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var playlist = await _playlistDal.GetWithSongsAsync(id);

            if (playlist is null || playlist.UserId != userId)
                return NotFound(new { Message = "Playlist bulunamadı." });

            var result = new
            {
                playlist.PlaylistId,
                playlist.Name,
                playlist.CoverImageUrl,
                playlist.Description,
                Songs = playlist.Songs.OrderBy(ps => ps.Order).Select(ps => new
                {
                    ps.Song.SongId,
                    ps.Song.Title,
                    ArtistName = ps.Song.Artist.Name,
                    ps.Song.CoverImageUrl,
                    Duration = ps.Song.Duration.TotalSeconds > 0
         ? $"{(int)ps.Song.Duration.TotalMinutes}:{ps.Song.Duration.Seconds:D2}"
         : null   
                })
            };

            return Ok(result);
        }
        [HttpPut("{id}/cover")]
        public async Task<IActionResult> UpdateCover(int id, IFormFile coverFile)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var playlist = await _playlistDal.GetByIdAsync(id);

            if (playlist is null || playlist.UserId != userId)
                return NotFound(new { Message = "Playlist bulunamadı." });

            var coversContainer = _configuration["AzureStorage:CoversContainer"]!;

            using (var stream = coverFile.OpenReadStream())
            {
                playlist.CoverImageUrl = await _blobStorageService.UploadFileAsync(stream, coverFile.FileName, coversContainer, coverFile.ContentType);
            }

            _playlistDal.Update(playlist);
            await _playlistDal.SaveChangesAsync();

            return Ok(new { playlist.PlaylistId, playlist.CoverImageUrl });
        }
    }
}
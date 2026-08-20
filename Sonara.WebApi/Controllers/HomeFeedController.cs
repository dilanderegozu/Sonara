using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sonara.DataAccessLayer.Repositories.Interfaces;
using Sonara.DtoLayer.Dtos.Playback;
using System.Security.Claims;

namespace Sonara.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class HomeFeedController : ControllerBase
    {
        private readonly ISongDal _songDal;
        private readonly IArtistDal _artistDal;
        private readonly IUserMembershipDal _userMembershipDal;
        private readonly IMembershipPlanDal _membershipPlanDal;
        private readonly IPlaylistDal _playlistDal;
        private readonly IPlaybackHistoryDal _playbackHistoryDal;
        private readonly IMoodDal _moodDal;

        public HomeFeedController(ISongDal songDal, IArtistDal artistDal, IUserMembershipDal userMembershipDal, IMembershipPlanDal membershipPlanDal, IPlaylistDal playlistDal, IPlaybackHistoryDal playbackHistoryDal, IMoodDal moodDal)
        {
            _songDal = songDal;
            _artistDal = artistDal;
            _userMembershipDal = userMembershipDal;
            _membershipPlanDal = membershipPlanDal;
            _playlistDal = playlistDal;
            _playbackHistoryDal = playbackHistoryDal;
            _moodDal = moodDal;
        }

        [HttpGet("all-artists")]
        public async Task<IActionResult> GetAllArtists()
        {
            var artists = await _artistDal.GetAllAsync();

            var result = artists.Select(a => new
            {
                a.ArtistId,
                a.Name,
                a.MonthlyListeners,
                a.ImageUrl
            });

            return Ok(result);
        }
        [HttpGet("all-songs")]
        public async Task<IActionResult> GetAllSongs()
        {
            var songs = await _songDal.GetAllWithArtistAsync();

            var result = songs.Select(s => new
            {
                s.SongId,
                s.Title,
                s.ArtistId,
                s.PlayCount,
                ArtistName = s.Artist.Name,
                s.CoverImageUrl
            });

            return Ok(result);
        }

        [HttpGet("artists/{artistId}/songs")]
        public async Task<IActionResult> GetSongsByArtist(int artistId)
        {
            var songs = await _songDal.GetByArtistIdAsync(artistId);
            var artist = await _artistDal.GetByIdAsync(artistId);

            if (artist is null) return NotFound();

            var result = new
            {
                artist.ArtistId,
                artist.Name,
                artist.ImageUrl,
                Songs = songs.Select(s => new
                {
                    s.SongId,
                    s.Title,
                    s.CoverImageUrl
                })
            };

            return Ok(result);
        }
        [HttpGet("moods")]
        public async Task<IActionResult> GetMoods()
        {
            var moods = await _moodDal.GetAllWithSongCountAsync();

            var result = moods.Select(m => new
            {
                m.MoodId,
                m.Name,
                m.ColorHex,
                SongCount = m.Songs.Count
            });

            return Ok(result);
        }
        [HttpGet("all-playlists")]
        public async Task<IActionResult> GetAllPlaylists()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null) return Unauthorized();

            var playlists = await _playlistDal.GetByUserIdAsync(userId);

            var result = playlists.Select(p => new
            {
                p.PlaylistId,
                p.Name,
                SongCount = p.Songs.Count,
                p.CoverImageUrl,
                p.CreatedDate
            });

            return Ok(result);
        }

        [HttpGet("continue-listening")]
        public async Task<IActionResult> GetContinueListening([FromQuery] int count = 4)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null) return Unauthorized();

            var history = await _playbackHistoryDal.GetRecentlyPlayedAsync(userId, count);

            var result = history.Select(h => new
            {
                h.SongId,
                h.Song.Title,
                ArtistName = h.Song.Artist.Name,
                h.Song.CoverImageUrl,   
                h.PositionSeconds,
                TotalSeconds = (int)h.Song.Duration.TotalSeconds,
                ProgressPercent = h.Song.Duration.TotalSeconds > 0
        ? (int)(h.PositionSeconds / h.Song.Duration.TotalSeconds * 100)
        : 0
            });
            return Ok(result);
        }

        [HttpPost("playback-progress")]
        public async Task<IActionResult> SaveProgress([FromBody] SaveProgressDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null) return Unauthorized();

            await _playbackHistoryDal.UpsertProgressAsync(userId, dto.SongId, dto.PositionSeconds);

            return Ok();
        }

        [HttpGet("recently-added")]
        public async Task<IActionResult> GetRecentlyAdded([FromQuery] int count = 8)
        {
            var songs = await _songDal.GetRecentlyAddedAsync(count);
            var result = songs.Select(s => new
            {
                s.SongId,
                s.Title,
                ArtistName = s.Artist.Name,
                s.CoverImageUrl
            });
            return Ok(result);
        }

        [HttpGet("popular-artists")]
        public async Task<IActionResult> GetPopularArtists([FromQuery] int count = 6)
        {
            var artists = await _artistDal.GetTopByListenersAsync(count);
            var result = artists.Select(a => new { a.ArtistId, a.Name, a.MonthlyListeners, a.ImageUrl });
            return Ok(result);
        }

        [HttpGet("my-membership")]
        public async Task<IActionResult> GetMyMembership()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null) return Unauthorized();

            var allPlans = await _membershipPlanDal.GetAllAsync();
            var maxLevel = allPlans.Any() ? allPlans.Max(p => p.Level) : 0;

            var activeMembership = await _userMembershipDal.GetActiveMembershipByUserIdAsync(userId);
            if (activeMembership is not null)
                return Ok(new { PlanName = activeMembership.MembershipPlan.Name, Level = activeMembership.MembershipPlan.Level, MaxLevel = maxLevel });

            var freePlan = await _membershipPlanDal.GetByNameAsync("Free");
            return Ok(new { PlanName = freePlan?.Name ?? "Free", Level = freePlan?.Level ?? 0, MaxLevel = maxLevel });
        }

        [HttpGet("my-playlists")]
        public async Task<IActionResult> GetMyPlaylists([FromQuery] int count = 6)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null) return Unauthorized();

            var playlists = await _playlistDal.GetByUserIdAsync(userId);

            var result = playlists.Take(count).Select(p => new
            {
                p.PlaylistId,
                p.Name,
                p.CoverImageUrl,
                SongCount = p.Songs.Count
            });

            return Ok(result);
        }
        [HttpGet("recommendations")]
        public async Task<IActionResult> GetRecommendations([FromQuery] int count = 5)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null) return Unauthorized();

            var songs = await _songDal.GetRecommendedForUserAsync(userId, count);

            var result = songs.Select(s => new
            {
                s.SongId,
                s.Title,
                ArtistName = s.Artist.Name,
                s.CoverImageUrl
            });

            return Ok(result);
        }

        [HttpGet("moods/{moodId}")]
        public async Task<IActionResult> GetMoodDetail(int moodId)
        {
            var mood = await _moodDal.GetWithSongsAsync(moodId);
            if (mood is null) return NotFound();

            var result = new
            {
                mood.MoodId,
                mood.Name,
                mood.ColorHex,
                Songs = mood.Songs.Select(sm => new
                {
                    sm.Song.SongId,
                    sm.Song.Title,
                    ArtistName = sm.Song.Artist.Name,
                    sm.Song.CoverImageUrl
                })
            };

            return Ok(result);
        }
    }
}

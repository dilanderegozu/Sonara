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

        public HomeFeedController(ISongDal songDal, IArtistDal artistDal, IUserMembershipDal userMembershipDal, IMembershipPlanDal membershipPlanDal, IPlaylistDal playlistDal, IPlaybackHistoryDal playbackHistoryDal)
        {
            _songDal = songDal;
            _artistDal = artistDal;
            _userMembershipDal = userMembershipDal;
            _membershipPlanDal = membershipPlanDal;
            _playlistDal = playlistDal;
            _playbackHistoryDal = playbackHistoryDal;
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
                SongCount = p.Songs.Count
            });

            return Ok(result);
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sonara.DataAccessLayer.Repositories.Interfaces;
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

        public HomeFeedController(ISongDal songDal, IArtistDal artistDal, IUserMembershipDal userMembershipDal, IMembershipPlanDal membershipPlanDal)
        {
            _songDal = songDal;
            _artistDal = artistDal;
            _userMembershipDal = userMembershipDal;
            _membershipPlanDal = membershipPlanDal;
        }

        [HttpGet("recently-added")]
        public async Task<IActionResult> GetRecentlyAdded([FromQuery] int count = 8)
        {
            var songs = await _songDal.GetRecentlyAddedAsync(count);
            var result = songs.Select(s => new { s.SongId, s.Title, ArtistName = s.Artist.Name });
            return Ok(result);
        }

        [HttpGet("popular-artists")]
        public async Task<IActionResult> GetPopularArtists([FromQuery] int count = 6)
        {
            var artists = await _artistDal.GetTopByListenersAsync(count);
            var result = artists.Select(a => new { a.ArtistId, a.Name, a.MonthlyListeners });
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
    }
}

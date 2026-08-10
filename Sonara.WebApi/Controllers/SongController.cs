using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sonara.DataAccessLayer.Repositories.Implementations;
using Sonara.DataAccessLayer.Repositories.Interfaces;
using System.Security.Claims;

namespace Sonara.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SongController : ControllerBase
    {
        private readonly ISongDal _songDal;
        private readonly IUserMembershipDal _userMembershipDal;
        private readonly IMembershipPlanDal _membershipPlanDal;

        public SongController(ISongDal songDal, IUserMembershipDal userMembershipDal, IMembershipPlanDal membershipPlanDal)
        {
            _songDal = songDal;
            _userMembershipDal = userMembershipDal;
            _membershipPlanDal = membershipPlanDal;
        }
        [HttpGet("{id}/play")]
        public async Task<IActionResult> Play(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
                return Unauthorized();

            var song = await _songDal.GetSongWithAllowedPlansAsync(id);
            if (song is null)
                return NotFound(new { Message = "Şarkı bulunamadı." });

            var activeMembership = await _userMembershipDal.GetActiveMembershipByUserIdAsync(userId);

            int currentPlanId;

            if (activeMembership is not null)
            {
                currentPlanId = activeMembership.MembershipPlanId;
            }
            else
            {
                var freePlan = await _membershipPlanDal.GetByNameAsync("Free");
                if (freePlan is null)
                    return StatusCode(500, new { Message = "Free plan tanımlı değil, sistem yapılandırması hatalı." });

                currentPlanId = freePlan.Id;
            }

            bool hasAccess = song.AllowedPlans.Any(x => x.MembershipPlanId == currentPlanId);

            if (!hasAccess)
                return StatusCode(403, new { Message = "Bu şarkıyı dinlemek için paketinizi yükseltin." });

            song.PlayCount++;
            _songDal.Update(song);
            await _songDal.SaveChangesAsync();

            return Ok(new { song.SongId, song.Title, song.AudioUrl });
        }
    }
}

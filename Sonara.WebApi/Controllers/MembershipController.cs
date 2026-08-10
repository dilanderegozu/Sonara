using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sonara.CoreLayer.Entities;
using Sonara.DataAccessLayer.Repositories.Interfaces;
using Sonara.DtoLayer.Dtos.Membership;
using System.Security.Claims;

namespace Sonara.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembershipController : ControllerBase
    {
        private readonly IMembershipPlanDal _membershipPlanDal;
        private readonly IUserMembershipDal _userMembershipDal;

        public MembershipController(IMembershipPlanDal membershipPlanDal, IUserMembershipDal userMembershipDal)
        {
            _membershipPlanDal = membershipPlanDal;
            _userMembershipDal = userMembershipDal;
        }

        [HttpPost("purchase")]
        public async Task<IActionResult> Purchase(PurchaseMembershipDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null) return Unauthorized();

            var plan = await _userMembershipDal.GetByIdAsync(dto.MembershipPlanId);
            if (plan is null) return BadRequest(new { Message = "Geçersiz paket." });

            var oldActiveMembership = await _userMembershipDal.GetAllActiveByUserIdAsync(userId);
            foreach (var old in  oldActiveMembership)
            {
                old.IsActive = false;
                old.EndDate = DateTime.UtcNow;
                _userMembershipDal.Update(old);
            }

            var newMembership = new UserMembership
            {
                UserId = userId,
                MembershipPlanId = plan.Id,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(plan.MembershipPlan.DurationInDays),
                IsActive = true
            };

            await _userMembershipDal.AddAsync(newMembership);
            await _userMembershipDal.SaveChangesAsync();

            return Ok(new { Message = $"{plan.MembershipPlan.Name} paketi başarıyla aktive edildi.", ExpiresAt = newMembership.EndDate });
        }
    }
}

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

        [HttpGet("plans")]
        public async Task<IActionResult> GetAllPlans()
        {
            var plans = await _membershipPlanDal.GetAllAsync();

            var result = plans.Select(p => new
            {
                p.Id,
                p.Name,
                p.Price,
                p.DurationInDays,
                p.MaxDeviceCount,
                p.HasAds,
                p.HasOfflineDownload,
                p.HasHighQualityAudio
            });

            return Ok(result);
        }
    }
}

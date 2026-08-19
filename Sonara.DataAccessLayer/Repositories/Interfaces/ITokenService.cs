using Sonara.CoreLayer.Entities;

namespace Sonara.CoreLayer.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateTokenAsync(ApplicationUser user, MembershipPlan? activePlan, IList<string> roles);
    }
}
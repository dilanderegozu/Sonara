using Sonara.DataAccessLayer.Repositories.Interfaces;

namespace Sonara.WebApi.BackgroundJobs
{
    public class MembershipExpirationJob
    {
        private readonly IUserMembershipDal _userMembershipDal;

        public MembershipExpirationJob(IUserMembershipDal userMembershipDal)
        {
            _userMembershipDal = userMembershipDal;
        }
        public async Task RunAsync()
        {
            var expiredMemberships = await _userMembershipDal.GetExpiredButStillActiveAsync();

            if (!expiredMemberships.Any())
                return;

            foreach (var membership in expiredMemberships)
            {
                membership.IsActive = false;
                _userMembershipDal.Update(membership);
            }

            await _userMembershipDal.SaveChangesAsync();
        }
    }
}

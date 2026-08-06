using Microsoft.EntityFrameworkCore;
using Sonara.CoreLayer.Entities;
using Sonara.DataAccessLayer.Context;
using Sonara.DataAccessLayer.Repositories.Interfaces;

namespace Sonara.DataAccessLayer.Repositories.Implementations
{
    public class MembershipPlanDal : GenericDal<MembershipPlan>, IMembershipPlanDal
    {
        public MembershipPlanDal(SonaraDbContext context, DbSet<MembershipPlan> dbSet) : base(context, dbSet)
        {
        }

        public async Task<MembershipPlan?> GetByNameAsync(string name)
            => await _context.MembershipPlans
                .FirstOrDefaultAsync(p => p.Name == name);
    }
}
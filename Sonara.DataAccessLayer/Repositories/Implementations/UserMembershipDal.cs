using Microsoft.EntityFrameworkCore;
using Sonara.CoreLayer.Entities;
using Sonara.DataAccessLayer.Context;
using Sonara.DataAccessLayer.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sonara.DataAccessLayer.Repositories.Implementations
{
    public class UserMembershipDal : GenericDal<UserMembership>, IUserMembershipDal
    {
        public UserMembershipDal(SonaraDbContext context) : base(context)
        {
        }

        public async Task<UserMembership?> GetActiveMembershipByUserIdAsync(string userId)
        {
           return await _context.UserMemberships.Include(u=>u.MembershipPlan).Where(x=>x.EndDate >DateTime.UtcNow && x.UserId== userId).OrderByDescending(um => um.EndDate).FirstOrDefaultAsync();
        }

        public async Task<List<UserMembership>> GetAllActiveByUserIdAsync(string userId)
        {
           return await _context.UserMemberships.Where(x=>x.UserId == userId && x.IsActive).ToListAsync();
        }

        public async Task<List<UserMembership>> GetExpiredButStillActiveAsync()
        {
            return await _context.UserMemberships.Where(um => um.IsActive && um.EndDate <= DateTime.UtcNow).ToListAsync();
        }
    }
}

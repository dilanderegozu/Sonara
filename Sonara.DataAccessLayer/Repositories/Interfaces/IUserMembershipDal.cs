using Sonara.CoreLayer.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sonara.DataAccessLayer.Repositories.Interfaces
{
    public interface IUserMembershipDal : IGenericDal<UserMembership>
    {
        Task<UserMembership?> GetActiveMembershipByUserIdAsync(string userId);
        Task<List<UserMembership>> GetExpiredButStillActiveAsync();
        Task<List<UserMembership>> GetAllActiveByUserIdAsync(string userId);
    }
}

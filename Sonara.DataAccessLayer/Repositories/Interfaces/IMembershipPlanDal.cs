using Sonara.CoreLayer.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sonara.DataAccessLayer.Repositories.Interfaces
{
    public interface IMembershipPlanDal : IGenericDal<MembershipPlan>
    {
        Task<MembershipPlan?> GetByNameAsync(string name);
    }
}

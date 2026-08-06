using System;
using System.Collections.Generic;
using System.Text;

namespace Sonara.CoreLayer.Entities
{
    public class UserMemberShip
    {
        public int Id { get; set; }

        public string UserId { get; set; }             
        public ApplicationUser User { get; set; }

        public int MembershipPlanId { get; set; }
        public MembershipPlan MembershipPlan { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}

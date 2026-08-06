using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sonara.CoreLayer.Entities
{
    public class ApplicationUser:IdentityUser
    {
        public DateTime RegisteredAt { get; set; }
        public ICollection<UserMembership> UserMemberships { get; set; }
    }
}

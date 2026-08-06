using System;
using System.Collections.Generic;
using System.Text;

namespace Sonara.CoreLayer.Entities
{
   public class MembershipPlan
{
    public int Id { get; set; }
    public string Name { get; set; }             
    public int MaxDeviceCount { get; set; }        
    public bool HasAds { get; set; }
    public bool HasOfflineDownload { get; set; }
    public bool HasHighQualityAudio { get; set; }
    public decimal Price { get; set; }
    public int DurationInDays { get; set; }        
    public ICollection<UserMemberShip> UserMemberships { get; set; }
}
}

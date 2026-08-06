namespace Sonara.CoreLayer.Entities
{
    public class SongMembershipPlan
    {
        public int SongId { get; set; }
        public Song Song { get; set; }
        public int MembershipPlanId { get; set; }
        public MembershipPlan MembershipPlan { get; set; }
    }
}
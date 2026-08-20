using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Sonara.CoreLayer.Entities;
namespace Sonara.DataAccessLayer.Context
{
    public class SonaraDbContext:IdentityDbContext<ApplicationUser>
    {
        public SonaraDbContext(DbContextOptions<SonaraDbContext> options) : base(options)
        {
        }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<MembershipPlan> MembershipPlans { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<SongMembershipPlan> SongMembershipPlans { get; set; }
        public DbSet<UserMembership> UserMemberships { get; set; }
        public DbSet<DeviceSession> DeviceSessions { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<PlaylistSong> PlaylistSongs { get; set; }
        public DbSet<PlaybackHistory> PlaybackHistories { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<SongMembershipPlan>()
                .HasKey(smp => new { smp.SongId, smp.MembershipPlanId });

            builder.Entity<SongMembershipPlan>()
              .HasOne(sp => sp.Song)
              .WithMany(s => s.AllowedPlans)
              .HasForeignKey(sp => sp.SongId);

            builder.Entity<SongMembershipPlan>()
                .HasOne(sp => sp.MembershipPlan)
                .WithMany()
                .HasForeignKey(sp => sp.MembershipPlanId);

            builder.Entity<MembershipPlan>()
            .Property(m => m.Price)
            .HasPrecision(10, 2);

            builder.Entity<PlaylistSong>()
                .HasKey(s => new { s.PlaylistId, s.SongId });

            builder.Entity<PlaylistSong>()
                .HasOne(s => s.Playlist)
                .WithMany(s => s.Songs)
                .HasForeignKey(s =>s.PlaylistId);

            builder.Entity<PlaylistSong>()
            .HasOne(ps => ps.Song)
            .WithMany()
            .HasForeignKey(ps => ps.SongId);

            builder.Entity<PlaybackHistory>()
    .HasIndex(ph => new { ph.UserId, ph.SongId })
    .IsUnique();
        }
    }
}

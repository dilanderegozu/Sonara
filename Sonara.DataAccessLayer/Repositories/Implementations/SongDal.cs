using Microsoft.EntityFrameworkCore;
using Sonara.CoreLayer.Entities;
using Sonara.DataAccessLayer.Context;
using Sonara.DataAccessLayer.Repositories.Interfaces;

namespace Sonara.DataAccessLayer.Repositories.Implementations
{
    public class SongDal : GenericDal<Song>, ISongDal
    {
        public SongDal(SonaraDbContext context) : base(context)
        {
        }

        public async Task AddAllowedPlansAsync(int songId, List<int> planIds)
        {
            foreach (var planId in planIds)
            {
                _context.SongMembershipPlans.Add(new SongMembershipPlan
                {
                    SongId = songId,
                    MembershipPlanId = planId
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<Song>> GetSongsByArtistIdAsync(int artistId)
            => await _context.Songs
                .Where(s => s.ArtistId == artistId)
                .ToListAsync();

        public async Task<Song?> GetSongWithAllowedPlansAsync(int songId)
            => await _context.Songs
                .Include(s => s.AllowedPlans)
                    .ThenInclude(sp => sp.MembershipPlan)
                .Include(s => s.Artist)
                .Include(s => s.Album)
                .FirstOrDefaultAsync(s => s.SongId == songId);

        public async Task<List<Song>> GetTopPlayedSongsAsync(int count)
            => await _context.Songs
                .OrderByDescending(s => s.PlayCount)
                .Take(count)
                .ToListAsync();

        public async Task<List<Song>> GetRecentlyAddedAsync(int count)
            => await _context.Songs
                .Include(s => s.Artist)
                .OrderByDescending(s => s.ReleaseDate)
                .Take(count)
                .ToListAsync();
    }
}
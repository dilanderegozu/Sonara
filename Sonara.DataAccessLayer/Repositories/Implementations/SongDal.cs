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

        public async Task AddMoodsAsync(int songId, List<int> moodIds)
        {
            foreach (var moodId in moodIds)
            {
                _context.SongMoods.Add(new SongMood { SongId = songId, MoodId = moodId });
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<Song>> GetRecommendedForUserAsync(string userId, int count)
        {
            // 1. Kullanıcının dinlediği şarkıların mood'larını bul (SongMoods tablosu üzerinden)
            var listenedSongIds = await _context.PlaybackHistories
                .Where(ph => ph.UserId == userId)
                .Select(ph => ph.SongId)
                .ToListAsync();

            var listenedMoodIds = await _context.SongMoods
                .Where(sm => listenedSongIds.Contains(sm.SongId))
                .Select(sm => sm.MoodId)
                .Distinct()
                .ToListAsync();

            List<Song> recommendations;

            if (listenedMoodIds.Any())
            {
                var candidateSongIds = await _context.SongMoods
                    .Where(sm => listenedMoodIds.Contains(sm.MoodId) && !listenedSongIds.Contains(sm.SongId))
                    .Select(sm => sm.SongId)
                    .Distinct()
                    .ToListAsync();

                recommendations = await _context.Songs
                    .Include(s => s.Artist)
                    .Where(s => candidateSongIds.Contains(s.SongId))
                    .OrderByDescending(s => s.PlayCount)
                    .Take(count)
                    .ToListAsync();
            }
            else
            {
                recommendations = new List<Song>();
            }

            if (recommendations.Count < count)
            {
                var excludeIds = recommendations.Select(s => s.SongId).Concat(listenedSongIds).ToList();

                var fallback = await _context.Songs
                    .Include(s => s.Artist)
                    .Where(s => !excludeIds.Contains(s.SongId))
                    .OrderByDescending(s => s.PlayCount)
                    .Take(count - recommendations.Count)
                    .ToListAsync();

                recommendations.AddRange(fallback);
            }

            return recommendations;
        }

        public async Task<List<Song>> GetAllWithArtistAsync()
        {
            return await _context.Songs.Include(s => s.Artist).ToListAsync();
        }

        public async Task<List<Song>> GetByArtistIdAsync(int artistId)
        {
            return await _context.Songs
                .Where(s => s.ArtistId == artistId)
                .ToListAsync();
        }
    }
}
using Microsoft.EntityFrameworkCore;
using Sonara.CoreLayer.Entities;
using Sonara.DataAccessLayer.Context;
using Sonara.DataAccessLayer.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sonara.DataAccessLayer.Repositories.Implementations
{
    public class PlaybackHistoryDal : GenericDal<PlaybackHistory>, IPlaybackHistoryDal
    {
        public PlaybackHistoryDal(SonaraDbContext context) : base(context)
        {
        }

        public async Task<List<PlaybackHistory>> GetRecentlyPlayedAsync(string userId, int count)
        {
           return await _context.PlaybackHistories.Where(s=>s.UserId == userId).Include(s=>s.Song).ThenInclude(s=>s.Artist).OrderByDescending(s=>s.LastPlayedAt).Take(count).ToListAsync();
        }

        public async Task UpsertProgressAsync(string userId, int songId, int positionSeconds)
        {
            var existing = await _context.PlaybackHistories.FirstOrDefaultAsync(s => s.UserId == userId && s.SongId == songId);
            if (existing != null)
            {
                existing.PositionSeconds = positionSeconds;
                existing.LastPlayedAt = DateTime.UtcNow;
            }else
            {
                await _context.PlaybackHistories.AddAsync(new PlaybackHistory { UserId = userId,SongId = songId, PositionSeconds = positionSeconds ,LastPlayedAt=DateTime.UtcNow});
            }

            await _context.SaveChangesAsync();
        }
    }
}

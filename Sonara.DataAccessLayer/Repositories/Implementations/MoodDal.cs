using Microsoft.EntityFrameworkCore;
using Sonara.CoreLayer.Entities;
using Sonara.DataAccessLayer.Context;
using Sonara.DataAccessLayer.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sonara.DataAccessLayer.Repositories.Implementations
{
    public class MoodDal : GenericDal<Mood>, IMoodDal
    {
        public MoodDal(SonaraDbContext context) : base(context)
        {
        }

        public async Task<List<Mood>> GetAllWithSongCountAsync()
        {
           return await _context.Moods.Include(s=>s.Songs).ToListAsync();
        }

        public async Task<Mood?> GetWithSongsAsync(int moodId)
        {
            return await _context.Moods.Include(s => s.Songs).ThenInclude(s => s.Song).ThenInclude(s => s.Artist).FirstOrDefaultAsync(m => m.MoodId == moodId);
        }
    }
}

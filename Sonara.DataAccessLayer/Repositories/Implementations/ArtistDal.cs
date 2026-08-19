using Microsoft.EntityFrameworkCore;
using Sonara.CoreLayer.Entities;
using Sonara.DataAccessLayer.Context;
using Sonara.DataAccessLayer.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sonara.DataAccessLayer.Repositories.Implementations
{
    public class ArtistDal : GenericDal<Artist>, IArtistDal
    {
        public ArtistDal(SonaraDbContext context) : base(context)
        {
        }

        public async Task<Artist?> GetArtistWithAlbumsAndSongsAsync(int artistId)
        {
            var values = await _context.Artists.Include(s=>s.Albums).Include(s=>s.Songs).FirstOrDefaultAsync(s => s.ArtistId == artistId);
            return values;
        }

        public async Task<List<Artist>> GetVerifiedArtistsAsync()
        {
            return await _context.Artists.Where(a => a.IsVerified).ToListAsync();
        }
        public async Task<List<Artist>> GetTopByListenersAsync(int count)
        {
            return await _context.Artists
             .OrderByDescending(a => a.MonthlyListeners)
             .Take(count)
             .ToListAsync();
        }
    }
}

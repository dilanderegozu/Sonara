using Microsoft.EntityFrameworkCore;
using Sonara.CoreLayer.Entities;
using Sonara.DataAccessLayer.Context;
using Sonara.DataAccessLayer.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sonara.DataAccessLayer.Repositories.Implementations
{
    public class PlaylistDal : GenericDal<Playlist>, IPlaylistDal
    {
        public PlaylistDal(SonaraDbContext context) : base(context)
        {
        }

        public async Task<List<Playlist>> GetByUserIdAsync(string userId)
        {
            return await _context.Playlists.Where(s=>s.UserId == userId).Include(a=>a.Songs).OrderByDescending(p=>p.CreatedDate).ToListAsync();
        }

        public async Task<Playlist?> GetWithSongsAsync(int playlistId)
        {
           return await _context.Playlists.Where(a=>a.PlaylistId == playlistId).Include(s=>s.Songs).ThenInclude(ps => ps.Song).ThenInclude(s => s.Artist).FirstOrDefaultAsync(p => p.PlaylistId == playlistId);
        }
    }
}

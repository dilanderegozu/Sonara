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
   
        public async Task AddSongAsync(int playlistId, int songId)
        {
            var maxOrder = await _context.PlaylistSongs
                .Where(ps => ps.PlaylistId == playlistId)
                .Select(ps => (int?)ps.Order)
                .MaxAsync() ?? 0;

            _context.PlaylistSongs.Add(new PlaylistSong
            {
                PlaylistId = playlistId,
                SongId = songId,
                AddedDate = DateTime.UtcNow,
                Order = maxOrder + 1
            });

            await _context.SaveChangesAsync();
        }

        public async Task RemoveSongAsync(int playlistId, int songId)
        {
            var entry = await _context.PlaylistSongs
                .FirstOrDefaultAsync(ps => ps.PlaylistId == playlistId && ps.SongId == songId);

            if (entry is not null)
            {
                _context.PlaylistSongs.Remove(entry);
                await _context.SaveChangesAsync();
            }
        }
    }
}

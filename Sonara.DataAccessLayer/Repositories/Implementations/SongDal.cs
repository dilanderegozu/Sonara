using Microsoft.EntityFrameworkCore;
using Sonara.CoreLayer.Entities;
using Sonara.DataAccessLayer.Context;
using Sonara.DataAccessLayer.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sonara.DataAccessLayer.Repositories.Implementations
{
    public class SongDal : GenericDal<Song>, ISongDal
    {
        public SongDal(SonaraDbContext context, DbSet<Song> dbSet) : base(context, dbSet)
        {
        }

        public async Task<List<Song>> GetSongsByArtistIdAsync(int artistId)
        {
           return await _context.Songs.Where(a=>a.ArtistId == artistId).ToListAsync();
        }

        public Task<Song?> GetSongWithAllowedPlansAsync(int songId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Song>> GetTopPlayedSongsAsync(int count)
        {
            throw new NotImplementedException();
        }
    }
}

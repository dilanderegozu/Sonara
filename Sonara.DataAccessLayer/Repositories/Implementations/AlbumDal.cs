using Microsoft.EntityFrameworkCore;
using Sonara.CoreLayer.Entities;
using Sonara.DataAccessLayer.Context;
using Sonara.DataAccessLayer.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sonara.DataAccessLayer.Repositories.Implementations
{
    public class AlbumDal : GenericDal<Album>, IAlbumDal
    {
        public AlbumDal(SonaraDbContext context, DbSet<Album> dbSet) : base(context, dbSet)
        {
        }

        public async Task<List<Album>> GetAlbumsByArtistIdAsync(int artistId)
        {
          var values = await _context.Albums.Where(s=>s.ArtistId == artistId).ToListAsync();
          return values;
        }

        public async Task<Album?> GetAlbumWithSongsAsync(int albumId)
        {
            return await _context.Albums.Include(a => a.Songs).FirstOrDefaultAsync(a => a.AlbumId == albumId);
        }
        }
    }

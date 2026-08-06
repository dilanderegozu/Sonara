using Sonara.CoreLayer.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sonara.DataAccessLayer.Repositories.Interfaces
{
    public interface IAlbumDal :IGenericDal<Album>
    {
        Task<List<Album>> GetAlbumsByArtistIdAsync(int artistId);
        Task<Album?> GetAlbumWithSongsAsync(int albumId);
    }
}

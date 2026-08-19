using Sonara.CoreLayer.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sonara.DataAccessLayer.Repositories.Interfaces
{
    public interface IArtistDal : IGenericDal<Artist>
    {
        Task<Artist?> GetArtistWithAlbumsAndSongsAsync(int artistId);
        Task<List<Artist>> GetVerifiedArtistsAsync();
        Task<List<Artist>> GetTopByListenersAsync(int count);
    }
}

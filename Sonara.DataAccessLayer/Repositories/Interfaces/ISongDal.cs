using Sonara.CoreLayer.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sonara.DataAccessLayer.Repositories.Interfaces
{
    public interface ISongDal: IGenericDal<Song>
    {
        Task<List<Song>> GetSongsByArtistIdAsync(int artistId);
        Task<Song?> GetSongWithAllowedPlansAsync(int songId);
        Task<List<Song>> GetTopPlayedSongsAsync(int count);
        Task<List<Song>> GetRecentlyAddedAsync(int count);
    }
}

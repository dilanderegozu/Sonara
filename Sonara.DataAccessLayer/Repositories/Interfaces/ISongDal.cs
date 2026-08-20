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
        Task AddAllowedPlansAsync(int songId, List<int> planIds);
        Task AddMoodsAsync(int songId, List<int> moodIds);
        Task<List<Song>> GetRecommendedForUserAsync(string userId, int count);
        Task<List<Song>> GetAllWithArtistAsync();
        Task<List<Song>> GetByArtistIdAsync(int artistId);
    }
}

using Sonara.CoreLayer.Entities;

namespace Sonara.DataAccessLayer.Repositories.Interfaces
{
    public interface IPlaylistDal : IGenericDal<Playlist>
    {
        Task<List<Playlist>> GetByUserIdAsync(string userId);
        Task<Playlist?> GetWithSongsAsync(int playlistId);
    }
}
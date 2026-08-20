using Sonara.CoreLayer.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sonara.DataAccessLayer.Repositories.Interfaces
{
    public interface IMoodDal :IGenericDal<Mood>
    {
        Task<List<Mood>> GetAllWithSongCountAsync();
        Task<Mood?> GetWithSongsAsync(int moodId);
    }
}

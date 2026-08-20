using Sonara.CoreLayer.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sonara.DataAccessLayer.Repositories.Interfaces
{
    public interface IPlaybackHistoryDal : IGenericDal<PlaybackHistory>
    {
        Task<List<PlaybackHistory>> GetRecentlyPlayedAsync(string userId, int count);
        Task UpsertProgressAsync(string userId, int songId, int positionSeconds);
    }
}

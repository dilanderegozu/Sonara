using Sonara.CoreLayer.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sonara.DataAccessLayer.Repositories.Interfaces
{
    public interface IDashboardDal
    {
        Task<int> GetTodayRegistrationCountAsync();
        Task<int> GetTodayPurchaseCountAsync();
        Task<decimal> GetTotalRevenueAsync();
        Task<List<Song>> GetTopPlayedSongsAsync(int count);
        Task<List<(Artist Artist, int TotalPlays)>> GetTopArtistsAsync(int count);
    }
}

using Microsoft.EntityFrameworkCore;
using Sonara.CoreLayer.Entities;
using Sonara.DataAccessLayer.Context;
using Sonara.DataAccessLayer.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sonara.DataAccessLayer.Repositories.Implementations
{
    public class DashboardDal :IDashboardDal
    {
        private readonly SonaraDbContext _context;

        public DashboardDal(SonaraDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetTodayPurchaseCountAsync()
        {
            var today = DateTime.UtcNow.Date;
            return await _context.UserMemberships.CountAsync(um => um.StartDate.Date == today);
        }

        public async Task<int> GetTodayRegistrationCountAsync()
        {
            var today = DateTime.UtcNow.Date;
            return await _context.Users.CountAsync(u=>u.RegisteredAt.Date == today);
        }

        public async Task<List<(Artist Artist, int TotalPlays)>> GetTopArtistsAsync(int count)
        {
            var result = await _context.Songs.Include(s => s.Artist).GroupBy(s => s.Artist).Select(a => new { Artist = a.Key, TotalPlays = a.Sum(s => s.PlayCount) })
               .OrderByDescending(x => x.TotalPlays)
               .Take(count)
               .ToListAsync();

            return result.Select(x => (x.Artist, (int)x.TotalPlays)).ToList();
        }

        public async Task<List<Song>> GetTopPlayedSongsAsync(int count)
        {
            return await _context.Songs.OrderByDescending(s=>s.PlayCount).Take(count).ToListAsync();
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _context.UserMemberships.Include(um => um.MembershipPlan).SumAsync(um => um.MembershipPlan.Price);
        }
    }
}

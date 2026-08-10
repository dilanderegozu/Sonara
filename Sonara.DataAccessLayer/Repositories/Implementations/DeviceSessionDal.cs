using Microsoft.EntityFrameworkCore;
using Sonara.CoreLayer.Entities;
using Sonara.DataAccessLayer.Context;
using Sonara.DataAccessLayer.Repositories.Interfaces;

namespace Sonara.DataAccessLayer.Repositories.Implementations
{
    public class DeviceSessionDal : GenericDal<DeviceSession>, IDeviceSessionDal
    {
        public DeviceSessionDal(SonaraDbContext context) : base(context)
        {
        }

        public async Task<List<DeviceSession>> GetActiveSessionsByUserIdAsync(string userId)
            => await _context.DeviceSessions
                .Where(ds => ds.UserId == userId)
                .OrderByDescending(ds => ds.LastActivityDate)
                .ToListAsync();

        public async Task<DeviceSession?> GetByDeviceIdentifierAsync(string userId, string deviceIdentifier)
            => await _context.DeviceSessions
                .FirstOrDefaultAsync(ds => ds.UserId == userId && ds.DeviceIdentifier == deviceIdentifier);

        public async Task<DeviceSession?> GetOldestSessionAsync(string userId)
            => await _context.DeviceSessions
                .Where(ds => ds.UserId == userId)
                .OrderBy(ds => ds.LoginDate)
                .FirstOrDefaultAsync();
    }
}
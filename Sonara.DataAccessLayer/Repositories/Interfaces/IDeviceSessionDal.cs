using Sonara.CoreLayer.Entities;

namespace Sonara.DataAccessLayer.Repositories.Interfaces
{
    public interface IDeviceSessionDal : IGenericDal<DeviceSession>
    {
        Task<List<DeviceSession>> GetActiveSessionsByUserIdAsync(string userId);
        Task<DeviceSession?> GetByDeviceIdentifierAsync(string userId, string deviceIdentifier);
        Task<DeviceSession?> GetOldestSessionAsync(string userId);
    }
}
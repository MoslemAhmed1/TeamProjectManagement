using TeamProjectManagement.Domain.Entities;

namespace TeamProjectManagement.Application.Interfaces.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task AddRefreshTokenAsync(RefreshToken token);
        Task<RefreshToken?> GetRefreshTokenAsync(string token);

    }
}

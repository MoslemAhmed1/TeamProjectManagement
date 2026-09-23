using Microsoft.EntityFrameworkCore;
using TeamProjectManagement.Application.Interfaces.Repositories;
using TeamProjectManagement.Domain.Entities;
using TeamProjectManagement.Infrastructure.Context;

namespace TeamProjectManagement.Infrastructure.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _context;

        public RefreshTokenRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddRefreshTokenAsync(RefreshToken token)
        {
            await _context.RefreshTokens.AddAsync(token);
        }

        public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
        {
            return await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.TokenHash == token);
        }
    }
}

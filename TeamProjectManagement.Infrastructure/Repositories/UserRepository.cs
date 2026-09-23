using Microsoft.EntityFrameworkCore;
using TeamProjectManagement.Application.Interfaces.Repositories;
using TeamProjectManagement.Domain.Entities;
using TeamProjectManagement.Infrastructure.Context;

namespace TeamProjectManagement.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByIdAsync(Guid userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task<User?> FindByIdentifierAsync(string identifier)
        {
            var lowerIdent = identifier.Trim().ToLowerInvariant();
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username.ToLower() == lowerIdent || u.Email.ToLower() == lowerIdent);
        }

        public async Task<(bool usernameExists, bool emailExists)> ExistsByUsernameOrEmailAsync(string username, string email)
        {
            var matches = await _context.Users
                .AsNoTracking()
                .Where(u => u.Username.ToLower() == username.ToLower() || u.Email.ToLower() == email.ToLower())
                .Select(u => new { u.Username, u.Email })
                .ToListAsync();

            return (
                matches.Any(u => u.Username.ToLower() == username.ToLower()),
                matches.Any(u => u.Email.ToLower() == email.ToLower())
            );
        }

        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }
    }
}

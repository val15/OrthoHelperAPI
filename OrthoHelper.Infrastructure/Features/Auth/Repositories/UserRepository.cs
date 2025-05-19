using Microsoft.EntityFrameworkCore;
using OrthoHelper.Domain.Features.Auth.Entities;
using OrthoHelper.Domain.Features.Auth.Ports;
using OrthoHelper.Infrastructure.Features.Common.Persistence.DbContext;

namespace OrthoHelper.Infrastructure.Features.Auth.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApiDbContext _context;

        public UserRepository(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<User> CreateUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> GetUserByUsername(string username)
        {
            var t_users = await _context.Users.ToListAsync();
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);
        }
    }
}

using OrthoHelper.Domain.Features.Auth.Entities;

namespace OrthoHelper.Domain.Features.Auth.Ports
{
    public interface IUserRepository
    {
        Task<User> CreateUser(User user);
        Task<User?> GetUserByUsername(string username);
    }
}

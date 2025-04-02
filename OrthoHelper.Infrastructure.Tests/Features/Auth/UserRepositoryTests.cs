using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using OrthoHelper.Domain.Features.Auth.Entities;
using OrthoHelper.Infrastructure.Features.Auth.Repositories;
using OrthoHelper.Infrastructure.Features.Common.Persistence.DbContext;

namespace OrthoHelper.Infrastructure.Tests.Features.Auth
{

    public class UserRepositoryTests : IDisposable
    {
        private readonly ApiDbContext _context;
        private readonly UserRepository _repository;

        public UserRepositoryTests()
        {
            _context = DbContextFactory.Create();
            _repository = new UserRepository(_context);

            // Nettoyer la base entre chaque test
            _context.Database.EnsureDeleted();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public async Task CreateUser_Should_Persist_User()
        {
            // Arrange
            var user = new User { Username = "testuser", PasswordHash = "hash" };

            // Act
            var result = await _repository.CreateUser(user);

            // Assert
            var savedUser = await _context.Users.FirstOrDefaultAsync();
            savedUser.Should().BeEquivalentTo(user);
        }

        [Fact]
        public async Task GetUserByUsername_Should_Return_Correct_User()
        {
            // Arrange
            var expectedUser = new User { Username = "testuser" , PasswordHash = "hash" };
            _context.Users.Add(expectedUser);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetUserByUsername("testuser");

            // Assert
            result.Should().BeEquivalentTo(expectedUser);
        }
    }
}

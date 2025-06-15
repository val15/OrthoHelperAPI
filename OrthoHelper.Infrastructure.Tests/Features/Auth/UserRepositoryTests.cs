using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using OrthoHelper.Domain.Features.Auth.Entities;
using OrthoHelper.Infrastructure.Features.Auth.Repositories;
using OrthoHelper.Infrastructure.Features.Common.Persistence.DbContext;

namespace OrthoHelper.Infrastructure.Tests.Features.Auth
{

    /// <summary>
    /// Contains tests for the UserRepository, verifying its interaction with the database.
    /// These tests use an in-memory database to ensure they are fast, isolated, and do not
    /// depend on an external database server.
    /// </summary>
    public class UserRepositoryTests : IDisposable
    {
        // The in-memory database context and the repository instance under test.
        private readonly ApiDbContext _context;
        private readonly UserRepository _repository;

        public UserRepositoryTests()
        {
            // ARRANGE (Shared setup for all tests)
            // 1. A factory creates a new in-memory database context for each test class instance.
            _context = DbContextFactory.Create();
            _repository = new UserRepository(_context);

            // 2. Ensure the database is clean before each test runs.
            //    This prevents state from one test affecting another.
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }

        /// <summary>
        /// Function: CreateUser
        /// Scenario: When a valid User entity is provided.
        /// Expected Result: The user should be successfully added to the database.
        /// </summary>
        [Fact]
        public async Task CreateUser_WhenUserIsValid_ShouldAddUserToDatabase()
        {
            // ARRANGE
            // Create a new user entity to be saved.
            var user = new User { Username = "testuser", PasswordHash = "hash" };

            // ACT
            // Call the repository method to create the user.
            await _repository.CreateUser(user);

            // ASSERT
            // Verify directly against the database context that the user was saved correctly.
            var savedUser = await _context.Users.FirstOrDefaultAsync();
            savedUser.Should().NotBeNull();
            savedUser.Should().BeEquivalentTo(user);
        }

        /// <summary>
        /// Function: GetUserByUsername
        /// Scenario: When a user with the specified username exists in the database.
        /// Expected Result: Should return the correct User entity.
        /// </summary>
        [Fact]
        public async Task GetUserByUsername_WhenUserExists_ShouldReturnCorrectUser()
        {
            // ARRANGE
            // 1. Create a user and save it to the in-memory database to set up the test condition.
            var expectedUser = new User { Username = "testuser", PasswordHash = "hash" };
            _context.Users.Add(expectedUser);
            await _context.SaveChangesAsync();

            // ACT
            // Call the repository method to retrieve the user.
            var result = await _repository.GetUserByUsername("testuser");

            // ASSERT
            // Verify that the retrieved user is the one we saved earlier.
            result.Should().BeEquivalentTo(expectedUser);
        }

        /// <summary>
        /// Disposes the database context after all tests in the class have run,
        /// ensuring proper cleanup of resources.
        /// </summary>
        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}

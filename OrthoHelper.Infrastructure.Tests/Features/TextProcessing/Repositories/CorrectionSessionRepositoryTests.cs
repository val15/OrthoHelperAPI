using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using Moq;
using OrthoHelper.Domain.Features.Auth.Entities;
using OrthoHelper.Domain.Features.Auth.Ports;
using OrthoHelper.Domain.Features.Common.Ports;
using OrthoHelper.Domain.Features.TextCorrection.Entities;
using OrthoHelper.Infrastructure.Features.Common.Persistence.DbContext;
using OrthoHelper.Infrastructure.Features.TextProcessing.Repositories;


namespace OrthoHelper.Infrastructure.Tests.Features.TextProcessing.Repositories
{
    /// <summary>
    /// Contains tests for the CorrectionSessionRepository, verifying its data interaction logic
    /// using an in-memory database and mocked dependencies.
    /// </summary>
    public class CorrectionSessionRepositoryTests : IDisposable
    {
        // The in-memory database context and the repository instance under test.
        private readonly ApiDbContext _context;
        private readonly CorrectionSessionRepository _repository;

        // Mocks for dependencies that are not under test.
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;

        public CorrectionSessionRepositoryTests()
        {
            // ARRANGE (Shared setup for all tests)
            // 1. Configure the in-memory database. A unique name ensures a clean database for each test.
            var options = new DbContextOptionsBuilder<ApiDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDB_" + Guid.NewGuid())
                .Options;
            _context = new ApiDbContext(options);

            // 2. Mock the ICurrentUserService to simulate a logged-in user with ID 1.
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _currentUserServiceMock.Setup(s => s.UserId).Returns(1);
            _currentUserServiceMock.Setup(s => s.UserName).Returns("testUser");
            _currentUserServiceMock.Setup(s => s.IsAuthenticated).Returns(true);

            // 3. Mock the IUserRepository (though it's not directly used in these specific tests, it's a dependency).
            _userRepoMock = new Mock<IUserRepository>();
            _userRepoMock.Setup(r=>r.GetUserByUsername(It.IsAny<string>()))
                         .ReturnsAsync(new User { Id = 1, Username = "testUser" });

            // 4. Initialize the repository to be tested, injecting the context and mocked services.
            _repository = new CorrectionSessionRepository(_context, _userRepoMock.Object, _currentUserServiceMock.Object);
            // 5. Ensure the database is created for the tests.
            _context.Database.EnsureCreated();
        }

        /// <summary>
        /// Function: AddAsync
        /// Scenario: When a valid session entity is provided.
        /// Expected Result: The session should be correctly persisted to the database.
        /// </summary>
        [Fact]
        public async Task AddAsync_WhenSessionIsValid_ShouldPersistSessionToDatabase()
        {
            // ARRANGE
            // Create a session entity to be added.
            var session = Session.Create("textIncorrect1");
            session.OutputText = "textCorrected1";
            session.ModelName = "Ollama:Gemma3";

            // ACT
            // Call the repository method to add the session.
            await _repository.AddAsync(session);

            // ASSERT
            // Verify directly against the context that one item was successfully saved.
            _context.Messages.Should().HaveCount(1);
            var savedMessage = await _context.Messages.FirstAsync();
            savedMessage.InputText.Should().Be("textIncorrect1");
        }

        /// <summary>
        /// Function: GetSessionsAsync
        /// Scenario: When no sessions exist for the current user in the database.
        /// Expected Result: Should return an empty list.
        /// </summary>
        [Fact]
        public async Task GetSessionsAsync_WhenDatabaseIsEmpty_ShouldReturnEmptyList()
        {
            // ACT
            // Call the repository method to get sessions.
            var result = await _repository.GetSessionsAsync();

            // ASSERT
            // Verify that the result is an empty collection.
            result.Should().BeEmpty();
        }

        /// <summary>
        /// Function: GetSessionsAsync
        /// Scenario: When sessions exist for the current user in the database.
        /// Expected Result: Should return a list containing all sessions for that user.
        /// </summary>
        [Fact]
        public async Task GetSessionsAsync_WhenSessionsExistForUser_ShouldReturnAllUserSessions()
        {
            // ARRANGE
            // Seed the database with sessions for the current user (ID 1).
            await _repository.AddAsync(Session.Create("text1"));
            await _repository.AddAsync(Session.Create("text2"));
            await _repository.AddAsync(Session.Create("text3"));

            // ACT
            var result = await _repository.GetSessionsAsync();

            // ASSERT
            // Verify that the repository returns all three sessions.
            result.Should().HaveCount(3);
        }

        /// <summary>
        /// Function: DeleteAllUserCorrectionSessionsAsync
        /// Scenario: When sessions exist for the current user.
        /// Expected Result: Should delete all sessions for that user from the database.
        /// </summary>
        [Fact]
        public async Task DeleteAllUserCorrectionSessionsAsync_WhenSessionsExist_ShouldRemoveAllFromDatabase()
        {
            // ARRANGE
            // 1. Seed the database with sessions.
            await _repository.AddAsync(Session.Create("text1"));
            await _repository.AddAsync(Session.Create("text2"));

            // 2. Confirm the initial state.
            _context.Messages.Should().HaveCount(2);

            // ACT
            // Call the delete method.
            await _repository.DeleteAllUserCorrectionSessionsAsync();

            // ASSERT
            // Verify that the database is now empty for that user.
            var sessionsAfterDelete = await _repository.GetSessionsAsync();
            sessionsAfterDelete.Should().BeEmpty();
        }

        /// <summary>
        /// Disposes the database context after tests are run, ensuring a clean state for the next run.
        /// </summary>
        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}

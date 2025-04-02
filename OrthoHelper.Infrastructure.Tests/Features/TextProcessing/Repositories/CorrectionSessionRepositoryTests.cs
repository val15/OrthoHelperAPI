using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using OrthoHelper.Domain.Features.Auth.Entities;
using OrthoHelper.Domain.Features.Auth.Ports;
using OrthoHelper.Domain.Features.Common.Ports;
using OrthoHelper.Domain.Features.TextCorrection.Entities;
using OrthoHelper.Infrastructure.Features.Common.Persistence.DbContext;
using OrthoHelper.Infrastructure.Features.TextProcessing.Entities;
using OrthoHelper.Infrastructure.Features.TextProcessing.Repositories;


namespace OrthoHelper.Infrastructure.Tests.Features.TextProcessing.Repositories
{
    public class CorrectionSessionRepositoryTests : IDisposable
    {
        private readonly ApiDbContext _context;
        private readonly IUserRepository _userRepository;
        private readonly CorrectionSessionRepository _repository;
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;

        public CorrectionSessionRepositoryTests()
        {
            // 1. Configuration de la base en mémoire
            var options = new DbContextOptionsBuilder<ApiDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDB_" + Guid.NewGuid())
                .Options;

            _context = new ApiDbContext(options);

            // 2. Mock de UserRepository
            _userRepoMock = new Mock<IUserRepository>();
            _userRepoMock.Setup(r => r.GetUserByUsername(It.IsAny<string>()))
                       .ReturnsAsync(new User { Id = 1 });
            _userRepository = _userRepoMock.Object;

            // 3. Initialisation du repository à tester
            _repository = new CorrectionSessionRepository(_context, _userRepository, _currentUserServiceMock.Object);
        }
        [Fact]
        public async Task AddAsync_Should_Persist_Data_InMemory()
        {
            // Act
            var correctionSessionTemp = new CorrectionSession(
         id: Guid.NewGuid(),
                originalText: "je ve",
                correctedText: "je veux",
                diff: "x",
                createdAt: DateTime.UtcNow,
                status: CorrectionStatus.Completed// Méthode helper
      );

            await _repository.AddAsync(correctionSessionTemp);



            // Assert
            _context.Messages.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetCorrectionSessionsAsync_Should_Return_UserSessions_InMemory()
        {
            // Arrange
            var testUser = new User { Id = 1, Username = "testuser" };

            // Seed la base de test
            _context.Messages.AddRange(
                new Message { Id = 1, UserId = 1, InputText = "Text1",OutputText = "Text 1",Diff = "1", CreatedAt = DateTime.UtcNow },
                new Message { Id = 2, UserId = 1, InputText = "Text2",OutputText = "Text 2",Diff = "2", CreatedAt = DateTime.UtcNow },
                new Message { Id = 3, UserId = 2, InputText = "OtherUserText", OutputText = "Other User Text", Diff = " ", CreatedAt = DateTime.UtcNow }
            );
            await _context.SaveChangesAsync();

            _userRepoMock.Setup(r => r.GetUserByUsername("testuser"))
                       .ReturnsAsync(testUser);

            // Act
            var result = await _repository.GetCorrectionSessionsAsync();

            // Assert
            result.Should().HaveCount(2);
            result.Should().OnlyContain(x =>
                x.OriginalText == "Text1" || x.OriginalText == "Text2");
        }

        [Fact]
        public async Task GetCorrectionSessionsAsync_Should_Return_Empty_For_NoSessions_InMemory()
        {
            // Arrange
            var testUser = new User { Id = 1, Username = "testuser" };
            _userRepoMock.Setup(r => r.GetUserByUsername("testuser"))
                       .ReturnsAsync(testUser);

            // Act
            var result = await _repository.GetCorrectionSessionsAsync();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetCorrectionSessionsAsync_Should_Throw_When_UserNotFound_InMemory()
        {
            // Arrange
            _userRepoMock.Setup(r => r.GetUserByUsername("unknown"))
                       .ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _repository.GetCorrectionSessionsAsync());
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}

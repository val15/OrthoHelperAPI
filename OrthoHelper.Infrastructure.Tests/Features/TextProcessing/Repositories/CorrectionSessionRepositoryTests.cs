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
            _currentUserServiceMock = new Mock<ICurrentUserService>();

            // 3. Initialisation du repository à tester
            _repository = new CorrectionSessionRepository(_context, _userRepository, _currentUserServiceMock.Object);
        }


        [Fact]
        public async Task AddAsync_Should_Persist_Data_InMemory()
        {
            // Act
            var correctionSessionTemp = Session.Create("textIncorrect1");
            correctionSessionTemp.OutputText = "textCorrected1";
            await _repository.AddAsync(correctionSessionTemp);



            // Assert
            _context.Messages.Should().HaveCount(1);
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
        public async Task GetCorrectionSessionsAsync_Should_Return_CorrectionSessions_InMemory()
        {
            // Arrange
            var testUser = new User { Id = 1, Username = "testuser" };

            // Seed la base de test
            var correctionSessionTemp = Session.Create("textIncorrect1");
            correctionSessionTemp.OutputText = "textCorrected1";
            await _repository.AddAsync(correctionSessionTemp);
            await _repository.AddAsync(correctionSessionTemp);
            await _repository.AddAsync(correctionSessionTemp);

            // Act
            var result = await _repository.GetCorrectionSessionsAsync();

            // Assert
            result.Should().HaveCount(3);
            result.Should().OnlyContain(x =>
                x.InputText == "textIncorrect1" || x.InputText == "textIncorrect1");
        }

        [Fact]
        public async Task DeleteAllUserCorrectionSessionsAsyn_Should_Return_CorrectionSessions_InMemory()
        {
            // Arrange
            var testUser = new User { Id = 1, Username = "testuser" };

            // Seed la base de test
            var correctionSessionTemp = Session.Create("textIncorrect1");
            correctionSessionTemp.OutputText = "textCorrected1";
            await _repository.AddAsync(correctionSessionTemp);
            await _repository.AddAsync(correctionSessionTemp);
            await _repository.AddAsync(correctionSessionTemp);

            // Act
            var result = await _repository.GetCorrectionSessionsAsync();

            // Assert
            result.Should().HaveCount(3);
            

            // Act
              await _repository.DeleteAllUserCorrectionSessionsAsync();

             result = await _repository.GetCorrectionSessionsAsync();
            result.Should().HaveCount(0);
        }




        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}

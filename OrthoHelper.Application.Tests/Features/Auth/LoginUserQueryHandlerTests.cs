using FluentAssertions;
using Moq;
using OrthoHelper.Application.Features.Auth.Queries.LoginUser;
using OrthoHelper.Domain.Features.Auth.Entities;
using OrthoHelper.Domain.Features.Auth.Ports;
using System.Security.Claims;

namespace OrthoHelper.Application.Tests.Features.Auth
{
    public class LoginUserQueryHandlerTests
    {
        private readonly Mock<IUserRepository> _mockRepo;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly LoginUserQueryHandler _handler;
        private readonly User _testUser;

        public LoginUserQueryHandlerTests()
        {
            _mockRepo = new Mock<IUserRepository>();
            _mockTokenService = new Mock<ITokenService>();
            _handler = new LoginUserQueryHandler(_mockRepo.Object, _mockTokenService.Object);

            _testUser = new User
            {
                Id = 1,
                Username = "testuser",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctPassword")
            };
        }

        [Fact]
        public async Task Handle_Should_Return_Token_When_Credentials_Valid()
        {
            // Arrange
            var expectedToken = "generated.jwt.token";
            var query = new LoginUserQuery("testuser", "correctPassword");

            _mockRepo.Setup(r => r.GetUserByUsername("testuser"))
                    .ReturnsAsync(_testUser);

            _mockTokenService.Setup(t => t.GenerateToken(It.IsAny<Claim[]>()))
                            .Returns(expectedToken);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result!.Token.Should().Be(expectedToken);

            _mockTokenService.Verify(t => t.GenerateToken(It.Is<Claim[]>(claims =>
                claims.Any(c => c.Type == ClaimTypes.NameIdentifier && c.Value == "1") &&
                claims.Any(c => c.Type == ClaimTypes.Name && c.Value == "testuser")
            )), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Return_Null_When_User_Not_Found()
        {
            // Arrange
            var query = new LoginUserQuery("unknown", "anypassword");
            _mockRepo.Setup(r => r.GetUserByUsername("unknown"))
                    .ReturnsAsync((User)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task Handle_Should_Return_Null_When_Wrong_Password()
        {
            // Arrange
            var query = new LoginUserQuery("testuser", "wrongPassword");
            _mockRepo.Setup(r => r.GetUserByUsername("testuser"))
                    .ReturnsAsync(_testUser);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
    }
}

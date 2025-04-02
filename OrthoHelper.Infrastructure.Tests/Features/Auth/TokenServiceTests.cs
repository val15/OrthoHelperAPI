using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using OrthoHelper.Infrastructure.Features.Auth.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace OrthoHelper.Infrastructure.Tests.Features.Auth
{
    public class TokenServiceTests
    {
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly TokenService _tokenService;

        public TokenServiceTests()
        {
            _mockConfig = new Mock<IConfiguration>();

            // Configuration mockée
            _mockConfig.Setup(c => c["Jwt:Key"]).Returns("SuperSecretKeyWithMinimum128BitsLength");

            _tokenService = new TokenService(_mockConfig.Object);
        }

        [Fact]
        public void GenerateToken_Should_Return_Valid_JWT()
        {
            // Arrange
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "testuser")
            };

            // Act
            var token = _tokenService.GenerateToken(claims);

            // Assert
            token.Should().NotBeNullOrEmpty();
            token.Split('.').Length.Should().Be(3); // Validation basique du format JWT
        }

        [Fact]
        public void GenerateToken_Should_Include_Claims()
        {
            // Arrange
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim("custom", "claim")
            };

            // Act
            var token = _tokenService.GenerateToken(claims);
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            // Assert
            jwt.Claims.Should().Contain(c =>
                c.Type == ClaimTypes.Name &&
                c.Value == "testuser");

            jwt.Claims.Should().Contain(c =>
                c.Type == "custom" &&
                c.Value == "claim");
        }

        [Fact]
        public void GenerateToken_Should_Throw_When_Key_Missing()
        {
            // Arrange
            var invalidConfig = new Mock<IConfiguration>();
            invalidConfig.Setup(c => c["Jwt:Key"]).Returns((string)null);
            var invalidService = new TokenService(invalidConfig.Object);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                invalidService.GenerateToken(Array.Empty<Claim>()));
        }
    }
}

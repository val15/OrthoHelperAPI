using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using OrthoHelper.Infrastructure.Features.Auth.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace OrthoHelper.Infrastructure.Tests.Features.Auth
{
    /// <summary>
    /// Contains unit tests for the TokenService, focusing on JWT generation and validation.
    /// </summary>
    public class TokenServiceTests
    {
        // Mock for the IConfiguration dependency
        private readonly Mock<IConfiguration> _mockConfig;

        // The service instance under test
        private readonly TokenService _tokenService;

        public TokenServiceTests()
        {
            _mockConfig = new Mock<IConfiguration>();

            // Mock the configuration to provide a valid JWT secret key.
            // This key must be long enough to satisfy the security algorithm's requirements.
            _mockConfig.Setup(c => c["Jwt:Key"]).Returns("ThisIsAValidSuperSecretKeyForTesting123!");

            _tokenService = new TokenService(_mockConfig.Object);
        }

        /// <summary>
        /// Function: GenerateToken
        /// Scenario: When provided with a set of claims and a valid configuration.
        /// Expected Result: Should return a string in the valid JWT format (header.payload.signature).
        /// </summary>
        [Fact]
        public void GenerateToken_WithValidClaims_ShouldReturnValidJwtFormat()
        {
            // ARRANGE
            // Define a set of claims to be included in the token.
            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Name, "testuser")
        };

            // ACT
            // Generate the token.
            var token = _tokenService.GenerateToken(claims);

            // ASSERT
            // 1. Verify that the token is not null or empty.
            token.Should().NotBeNullOrEmpty();

            // 2. Perform a basic format check to ensure it has the three parts of a JWT.
            token.Split('.').Length.Should().Be(3);
        }

        /// <summary>
        /// Function: GenerateToken
        /// Scenario: When provided with a set of claims.
        /// Expected Result: The generated JWT should contain all the provided claims.
        /// </summary>
        [Fact]
        public void GenerateToken_WithValidClaims_ShouldIncludeAllClaimsInToken()
        {
            // ARRANGE
            // Define a specific set of claims to look for after decoding.
            var claims = new[]
            {
            new Claim(ClaimTypes.Name, "testuser"),
            new Claim("custom", "claimValue")
        };

            // ACT
            // 1. Generate the token.
            var token = _tokenService.GenerateToken(claims);

            // 2. Decode the token to inspect its contents.
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            // ASSERT
            // Verify that the decoded claims collection contains the exact claims we provided.
            jwt.Claims.Should().Contain(c => c.Type == ClaimTypes.Name && c.Value == "testuser");
            jwt.Claims.Should().Contain(c => c.Type == "custom" && c.Value == "claimValue");
        }

        /// <summary>
        /// Function: GenerateToken
        /// Scenario: When the JWT secret key is missing from the configuration.
        /// Expected Result: Should throw an ArgumentNullException.
        /// </summary>
        [Fact]
        public void GenerateToken_WhenJwtKeyIsMissing_ShouldThrowArgumentNullException()
        {
            // ARRANGE
            // 1. Create a separate, invalid configuration mock where the key is null.
            var invalidConfig = new Mock<IConfiguration>();
            invalidConfig.Setup(c => c["Jwt:Key"]).Returns((string)null);

            // 2. Create a new service instance with the invalid configuration.
            var invalidService = new TokenService(invalidConfig.Object);

            // 3. Define the action that should fail.
            Action act = () => invalidService.GenerateToken(Array.Empty<Claim>());

            // ACT & ASSERT
            // Verify that calling the method throws the expected exception.
            act.Should().Throw<ArgumentNullException>();
        }
    }
}

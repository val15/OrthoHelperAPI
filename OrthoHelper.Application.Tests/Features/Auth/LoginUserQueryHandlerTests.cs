using FluentAssertions;
using Moq;
using OrthoHelper.Application.Features.Auth.Queries.LoginUser;
using OrthoHelper.Domain.Features.Auth.Entities;
using OrthoHelper.Domain.Features.Auth.Ports;
using System.Security.Claims;

namespace OrthoHelper.Application.Tests.Features.Auth
{
    /// <summary>
    /// Contains unit tests for the LoginUserQueryHandler.
    /// It covers scenarios for successful login, user not found, and invalid password.
    /// </summary>
    public class LoginUserQueryHandlerTests
    {
        // Mocks for the dependencies of the handler
        private readonly Mock<IUserRepository> _mockRepo;
        private readonly Mock<ITokenService> _mockTokenService;

        // The instance of the class we are testing
        private readonly LoginUserQueryHandler _handler;

        // A reusable test user object for the tests
        private readonly User _testUser;

        /// <summary>
        /// The constructor sets up the shared resources for all tests in this class.
        /// This is where mocks and the handler instance are initialized.
        /// </summary>
        public LoginUserQueryHandlerTests()
        {
            // Initialize the mocks for the repository and token service.
            _mockRepo = new Mock<IUserRepository>();
            _mockTokenService = new Mock<ITokenService>();

            // Create an instance of the handler, injecting the mock dependencies.
            _handler = new LoginUserQueryHandler(_mockRepo.Object, _mockTokenService.Object);

            // Create a standard user for successful login scenarios.
            // The password "correctPassword" is hashed to simulate how it's stored in a real database.
            _testUser = new User
            {
                Id = 1,
                Username = "testuser",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctPassword")
            };
        }

        /// <summary>
        /// Tests that when valid credentials are provided, the handler returns a response
        /// containing a valid JWT.
        /// </summary>
        [Fact]
        public async Task Handle_WithValidCredentials_ShouldReturnTokenResponse()
        {
            // ARRANGE
            // 1. Define the expected token that our mock token service will return.
            var expectedToken = "generated.jwt.token";

            // 2. Create the query with the correct username and password.
            var query = new LoginUserQuery("testuser", "correctPassword");

            // 3. Setup the mock repository to return our test user when queried by username.
            _mockRepo.Setup(r => r.GetUserByUsername("testuser"))
                     .ReturnsAsync(_testUser);

            // 4. Setup the mock token service to return the expected token.
            _mockTokenService.Setup(t => t.GenerateToken(It.IsAny<Claim[]>()))
                             .Returns(expectedToken);

            // ACT
            // Execute the handler with the login query.
            var result = await _handler.Handle(query, CancellationToken.None);

            // ASSERT
            // 1. Ensure the result is not null.
            result.Should().NotBeNull();

            // 2. Verify that the token in the result matches our expected token.
            result!.Token.Should().Be(expectedToken);

            // 3. Verify that the token service's GenerateToken method was called exactly once
            //    with the correct user ID and username claims.
            _mockTokenService.Verify(t => t.GenerateToken(It.Is<Claim[]>(claims =>
                claims.Any(c => c.Type == ClaimTypes.NameIdentifier && c.Value == "1") &&
                claims.Any(c => c.Type == ClaimTypes.Name && c.Value == "testuser")
            )), Times.Once);
        }

        /// <summary>
        /// Tests that the handler returns null when the provided username does not exist
        /// in the repository.
        /// </summary>
        [Fact]
        public async Task Handle_WhenUserNotFound_ShouldReturnNull()
        {
            // ARRANGE
            // 1. Create a query for a user that does not exist.
            var query = new LoginUserQuery("unknown", "anypassword");

            // 2. Setup the mock repository to return null, simulating a user not being found.
            _mockRepo.Setup(r => r.GetUserByUsername("unknown"))
                     .ReturnsAsync((User)null);

            // ACT
            // Execute the handler.
            var result = await _handler.Handle(query, CancellationToken.None);

            // ASSERT
            // Verify that the result is null.
            result.Should().BeNull();
        }

        /// <summary>
        /// Tests that the handler returns null when the username is correct but the
        /// password is incorrect.
        /// </summary>
        [Fact]
        public async Task Handle_WithInvalidPassword_ShouldReturnNull()
        {
            // ARRANGE
            // 1. Create a query with a valid username but a wrong password.
            var query = new LoginUserQuery("testuser", "wrongPassword");

            // 2. Setup the mock repository to return the test user. The handler should
            //    then fail at the password verification step.
            _mockRepo.Setup(r => r.GetUserByUsername("testuser"))
                     .ReturnsAsync(_testUser);

            // ACT
            // Execute the handler.
            var result = await _handler.Handle(query, CancellationToken.None);

            // ASSERT
            // Verify that the result is null because the password check failed.
            result.Should().BeNull();
        }
    }
}

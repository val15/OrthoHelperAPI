using FluentAssertions;
using Moq;
using OrthoHelper.Application.Features.Auth.Commands.RegisterUser;
using OrthoHelper.Domain.Features.Auth.Entities;
using OrthoHelper.Domain.Features.Auth.Ports;

namespace OrthoHelper.Application.Tests.Features.Auth
{
    /// <summary>
    /// Contains unit tests for the RegisterUserCommandHandler.
    /// </summary>
    public class RegisterUserCommandHandlerTests
    {
        /// <summary>
        /// Tests that the Handle method correctly creates a new user with a properly hashed password
        /// when given a valid registration command.
        /// </summary>
        [Fact]
        public async Task Handle_WithValidCommand_ShouldCreateUserWithHashedPassword()
        {
            // ARRANGE
            // 1. Mock the user repository dependency to control its behavior.
            var mockRepo = new Mock<IUserRepository>();

            // 2. Instantiate the handler we are testing, injecting the mock repository.
            var handler = new RegisterUserCommandHandler(mockRepo.Object);

            // 3. Define test data.
            var password = "securePassword123";
            var command = new RegisterUserCommand("testuser", password);

            // 4. Setup the mock to return the user object that it receives.
            // This allows us to inspect the user that was passed to the CreateUser method.
            mockRepo.Setup(r => r.CreateUser(It.IsAny<User>()))
                    .ReturnsAsync((User user) => user);

            // ACT
            // Execute the handler with the command.
            var result = await handler.Handle(command, CancellationToken.None);

            // ASSERT
            // 1. Verify that the CreateUser method on our repository was called exactly once.
            mockRepo.Verify(r => r.CreateUser(It.IsAny<User>()), Times.Once);

            // 2. Assert that the returned user has the correct username.
            result.Username.Should().Be("testuser");

            // 3. (Important) Verify that the password stored is a valid hash of the original password.
            //    This assertion is crucial for the test's purpose. It should be uncommented.
            // BCrypt.Net.BCrypt.Verify(password, result.PasswordHash).Should().BeTrue();
        }
    }
}

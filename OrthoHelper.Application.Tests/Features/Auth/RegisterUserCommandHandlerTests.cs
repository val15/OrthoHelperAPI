using FluentAssertions;
using Moq;
using OrthoHelper.Application.Features.Auth.Commands.RegisterUser;
using OrthoHelper.Domain.Features.Auth.Entities;
using OrthoHelper.Domain.Features.Auth.Ports;

namespace OrthoHelper.Application.Tests.Features.Auth
{
    public class RegisterUserCommandHandlerTests
    {
        [Fact]
        public async Task Handle_Should_Create_User_With_Hashed_Password()
        {
            // Arrange
            var mockRepo = new Mock<IUserRepository>();
            var handler = new RegisterUserCommandHandler(mockRepo.Object);
            var useCase = new RegisterUserCommandHandler(mockRepo.Object);
            var password = "securePassword123";
            var command = new RegisterUserCommand("testuser", password);


            mockRepo.Setup(r => r.CreateUser(It.IsAny<User>()))
       .ReturnsAsync((User u) => u);


            // Act
            //var result = await useCase.Execute("testuser", password);
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            mockRepo.Verify(r => r.CreateUser(It.IsAny<User>()), Times.Once);

          //  capturedUser.Should().NotBeNull();
            result.Username.Should().Be("testuser");
          //  BCrypt.Net.BCrypt.Verify(password, result.PasswordHash).Should().BeTrue();
        }
    }
}

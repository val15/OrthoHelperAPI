using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OrthoHelper.Application.Features.Auth.Commands.RegisterUser;
using OrthoHelper.Application.Features.Auth.DTOs;
using OrthoHelper.Application.Features.Auth.Queries.LoginUser;
using OrthoHelperAPI.Controllers;

namespace OrthoHelperAPI.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new AuthController(_mockMediator.Object);
        }

        [Fact]
        public async Task Register_Should_Send_RegisterCommand_And_Return_CreatedResult()
        {
            // Arrange
            var dto = new RegisterDto { Username = "testuser", Password = "Password123!" };
            var expectedResponse = new RegisterUserResponse(1, "testuser");

            _mockMediator.Setup(m => m.Send(It.IsAny<RegisterUserCommand>(), default))
                        .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.Register(dto);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
            var createdAtResult = result as CreatedAtActionResult;
            createdAtResult.Value.Should().BeEquivalentTo(expectedResponse);

            _mockMediator.Verify(m => m.Send(
                It.Is<RegisterUserCommand>(c =>
                    c.Username == dto.Username &&
                    c.Password == dto.Password),
                default),
                Times.Once);
        }

        [Fact]
        public async Task Login_Should_Return_Token_When_Credentials_Valid()
        {
            // Arrange
            var dto = new LoginDto { Username = "testuser", Password = "Password123!" };
            var expectedToken = new LoginUserResponse("fake.jwt.token");

            _mockMediator.Setup(m => m.Send(It.IsAny<LoginUserQuery>(), default))
                        .ReturnsAsync(expectedToken);

            // Act
            var result = await _controller.Login(dto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(expectedToken);
        }

        [Fact]
        public async Task Login_Should_Return_Unauthorized_When_Credentials_Invalid()
        {
            // Arrange
            var dto = new LoginDto { Username = "testuser", Password = "WrongPassword" };

            _mockMediator.Setup(m => m.Send(It.IsAny<LoginUserQuery>(), default))
                        .ReturnsAsync((LoginUserResponse)null);

            // Act
            var result = await _controller.Login(dto);

            // Assert
            result.Should().BeOfType<UnauthorizedResult>();
        }
    }
}

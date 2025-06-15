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
    /// <summary>
    /// Contains unit tests for the AuthController, focusing on its interaction with MediatR
    /// to handle user registration and login commands and queries.
    /// </summary>
    public class AuthControllerTests
    {
        // Mock for the MediatR dependency
        private readonly Mock<IMediator> _mockMediator;

        // The controller instance under test
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new AuthController(_mockMediator.Object);
        }

        /// <summary>
        /// Function: Register (POST)
        /// Scenario: When provided with valid user registration data.
        /// Expected Result: Should send a RegisterUserCommand and return a CreatedAtActionResult.
        /// </summary>
        [Fact]
        public async Task Register_WithValidCredentials_ShouldReturnCreatedAtResult()
        {
            // ARRANGE
            // 1. Define the input data (DTO) and the expected successful response from the mediator.
            var dto = new RegisterDto { Username = "testuser", Password = "Password123!" };
            var expectedResponse = new RegisterUserResponse(1, "testuser");

            // 2. Configure the mediator mock to return the successful response when the command is sent.
            _mockMediator.Setup(m => m.Send(It.IsAny<RegisterUserCommand>(), default))
                         .ReturnsAsync(expectedResponse);

            // ACT
            // Call the controller's Register action.
            var result = await _controller.Register(dto);

            // ASSERT
            // 1. Verify that the result is an HTTP 201 CreatedAtAction response.
            var createdAtResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;

            // 2. Verify that the content of the result matches the expected response.
            createdAtResult.Value.Should().BeEquivalentTo(expectedResponse);

            // 3. Verify that MediatR's Send method was called exactly once with the correct command data.
            _mockMediator.Verify(m => m.Send(
                It.Is<RegisterUserCommand>(c =>
                    c.Username == dto.Username && c.Password == dto.Password),
                default),
                Times.Once);
        }

        /// <summary>
        /// Function: Login (POST)
        /// Scenario: When provided with valid user credentials.
        /// Expected Result: Should return an OkObjectResult containing a JWT.
        /// </summary>
        [Fact]
        public async Task Login_WithValidCredentials_ShouldReturnOkWithToken()
        {
            // ARRANGE
            // 1. Define the login DTO and the expected successful token response.
            var dto = new LoginDto { Username = "testuser", Password = "Password123!" };
            var expectedResponse = new LoginUserResponse("fake.jwt.token");

            // 2. Configure the mediator to return the token response.
            _mockMediator.Setup(m => m.Send(It.IsAny<LoginUserQuery>(), default))
                         .ReturnsAsync(expectedResponse);

            // ACT
            var result = await _controller.Login(dto);

            // ASSERT
            // 1. Verify the result is an HTTP 200 OK response.
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;

            // 2. Verify the content of the result matches the expected token response.
            okResult.Value.Should().BeEquivalentTo(expectedResponse);
        }

        /// <summary>
        /// Function: Login (POST)
        /// Scenario: When provided with invalid user credentials.
        /// Expected Result: Should return an UnauthorizedResult.
        /// </summary>
    //    [Fact]
    //    public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
    //    {
    //        // ARRANGE
    //        // 1. Define the login DTO with incorrect credentials.
    //        var dto = new LoginDto { Username = "testuser", Password = "WrongPassword" };

    //        // 2. Configure the mediator to return null, simulating a failed login attempt.
    //        _mockMediator.Setup(m => m.Send(It.IsAny<LoginUserQuery>(), default))
    //                     .ReturnsAsync((LoginUserResponse)null);

    //        // ACT
    //        var result = await _controller.Login(dto);

    //        // ASSERT
    //        // Verify that the result is an HTTP 401 Unauthorized response.
    //        result.Should().BeOfType<UnauthorizedResult>();
    //    }
    }
}

using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using OrthoHelperAPI.Controllers;
using OrthoHelperAPI.Data;
using OrthoHelperAPI.Model;
using OrthoHelperAPI.Repositories;
using OrthoHelperAPI.Services.Interfaces;
using System.Security.Claims;

namespace OrthoHelperAPI.Tests
{
    /// <summary>
    /// Contains unit tests for the TextOLDController. These tests focus on the controller's logic in isolation,
    /// using an in-memory database and mocked services to simulate dependencies.
    /// </summary>
    public class TextControllerTests : IDisposable
    {
        // Mocks for the controller's dependencies
        private readonly Mock<ITextProcessingService> _textProcessingServiceMock;
        private readonly Mock<IMessageRepository> _messageRepositoryMock;

        // The controller instance being tested
        private readonly TextOLDController _controller;

        // An in-memory database context for the tests
        private readonly ApiDbContext _context;

        public TextControllerTests()
        {
            // ARRANGE (Shared setup for all tests)

            // 1. Set up the in-memory database. A unique name ensures a clean database for each test run.
            var options = new DbContextOptionsBuilder<ApiDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApiDbContext(options);

            // 2. Initialize mocks for the service and repository dependencies.
            _textProcessingServiceMock = new Mock<ITextProcessingService>();
            _messageRepositoryMock = new Mock<IMessageRepository>();

            // 3. Create the controller instance, injecting the mocked dependencies.
            _controller = new TextOLDController(_messageRepositoryMock.Object, _textProcessingServiceMock.Object);

            // 4. Simulate an authenticated user by creating a fake ClaimsPrincipal.
            //    This is necessary to test controller actions that require authorization
            //    and access user information like the user ID.
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.NameIdentifier, "1") // Fake user with ID "1"
            }, "mock-authentication"));

            // 5. Assign the fake user to the controller's context.
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        /// <summary>
        /// Function: GetUserMessages
        /// Scenario: When an authenticated user requests their messages.
        /// Expected Result: Returns HTTP 200 OK with the user's list of messages.
        /// </summary>
        [Fact]
        public async Task GetUserMessages_WhenUserIsAuthenticated_ReturnsOkWithUserMessages()
        {
            // ARRANGE
            // 1. Define the user ID, which should match the one in the fake ClaimsPrincipal.
            var userId = 1;

            // 2. Create a fake list of messages to be returned by the mock repository.
            var messages = new List<Message>
        {
            new Message { Id = 1, InputText = "Test1", OutputText = "Corrected1", UserId = userId },
            new Message { Id = 2, InputText = "Test2", OutputText = "Corrected2", UserId = userId }
        };

            // 3. Configure the mock repository to return the fake list when GetUserMessagesAsync is called.
            _messageRepositoryMock.Setup(repo => repo.GetUserMessagesAsync(userId))
                                  .ReturnsAsync(messages);

            // ACT
            // Call the controller action.
            var result = await _controller.GetUserMessages();

            // ASSERT
            // 1. Verify that the result is an HTTP 200 OK response.
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;

            // 2. Verify that the value of the OK result is a list of Message objects.
            var returnedMessages = okResult.Value.Should().BeOfType<List<Message>>().Subject;

            // 3. Verify that the list contains the correct number of messages.
            returnedMessages.Should().HaveCount(2);

            // 4. Verify that the GetUserMessagesAsync method on the repository was called exactly once with the correct user ID.
            _messageRepositoryMock.Verify(repo => repo.GetUserMessagesAsync(userId), Times.Once);
        }

        /// <summary>
        /// Cleans up resources after all tests in the class have run by deleting the in-memory database.
        /// </summary>
        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
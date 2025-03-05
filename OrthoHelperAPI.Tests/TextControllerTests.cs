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
    public class TextControllerTests
    {
        private readonly ApiDbContext _context;
        private readonly Mock<ITextProcessingService> _textProcessingServiceMock;
        private readonly Mock<IMessageRepository> _messageRepositoryMock;
        private readonly TextController _controller;

        public TextControllerTests()
        {
            // Setup DbContext
            var options = new DbContextOptionsBuilder<ApiDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new ApiDbContext(options);

            // Setup mocks
            _textProcessingServiceMock = new Mock<ITextProcessingService>();
            _messageRepositoryMock = new Mock<IMessageRepository>();

            // Create controller with mocked dependencies
            _controller = new TextController(_messageRepositoryMock.Object, _textProcessingServiceMock.Object);

            // Configure fake user claims
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task GetUserMessages_ReturnsMessagesForUser()
        {
            // Arrange
            var userId = 1;
            var messages = new List<Message>
            {
                new Message { Id = 1, InputText = "Test1", OutputText = "Test1", Diff = "d1", UserId = userId },
                new Message { Id = 2, InputText = "Test2", OutputText = "Test2", Diff = "d2", UserId = userId }
            };

            _messageRepositoryMock.Setup(repo => repo.GetUserMessagesAsync(userId))
                .ReturnsAsync(messages);

            // Act
            var result = await _controller.GetUserMessages();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnMessages = Assert.IsType<List<Message>>(okResult.Value);
            Assert.Equal(2, returnMessages.Count);
            _messageRepositoryMock.Verify(repo => repo.GetUserMessagesAsync(userId), Times.Once);
        }

      
       
        // Cleanup after tests if needed
        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
using OrthoHelper.Infrastructure.TextProcessing;
using Moq;
using System.Threading;
using OrthoHelper.Domain.Ports;

namespace OrthoHelper.Infrastructure.Tests
{

    // OrthoHelper.Infrastructure.Tests/OrthoEngineAdapterTests.cs
    public class OrthoEngineAdapterTests
    {
        [Fact]
        public async Task CorrectTextAsync_ValidInput_ReturnsCorrectedText()
        {
            // Arrange
            var mockEngine = new Mock<IOrthoEngine>();
            mockEngine.Setup(e => e.ProcessTextAsync("test")).ReturnsAsync("TEST");

            var adapter = new OrthoEngineAdapter(mockEngine.Object);

            // Act
            var result = await adapter.CorrectTextAsync("test");

            // Assert
            Assert.Equal("TEST", result);
        }
    }
}
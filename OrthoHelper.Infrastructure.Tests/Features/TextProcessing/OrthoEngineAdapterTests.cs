using Moq;
using OrthoHelper.Infrastructure.Features.TextProcessing;

namespace OrthoHelper.Infrastructure.Tests.Features.TextProcessing
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
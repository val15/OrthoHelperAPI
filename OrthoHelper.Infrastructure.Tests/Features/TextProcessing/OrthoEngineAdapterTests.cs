using Moq;
using OrthoHelper.Domain.Features.TextCorrection.Ports;
using OrthoHelper.Infrastructure.Features.TextProcessing;

namespace OrthoHelper.Infrastructure.Tests.Features.TextProcessing
{

    // OrthoHelper.Infrastructure.Tests/OrthoEngineAdapterTests.cs
    public class OrthoEngineAdapterTests
    {
        [Fact]
        public async Task CorrectTextAsync_ValidInput_ReturnsOutputText()
        {
            // Arrange
            var mockEngine = new Mock<IOrthoEngine>();
            mockEngine.Setup(e => e.ProcessTextAsync("test")).ReturnsAsync("TEST");

            var adapter = new OrthoEngineAdapter(mockEngine.Object);

            // Act
            var result = await adapter.ProcessTextAsync("test");

            // Assert
            Assert.Equal("TEST", result);
        }
    }
}
// OrthoHelper.Domain.Tests/Services/CorrectionOrchestratorTests.cs
using FluentAssertions;
using Moq;
using OrthoHelper.Domain.Ports;
using OrthoHelper.Domain.Services;
using OrthoHelper.Domain.Entities;
using Xunit;
using OrthoHelper.Domain.Exceptions;
namespace OrthoHelper.Domain.Tests.Services
{


    public class CorrectionOrchestratorTests
    {
        private readonly Mock<ITextProcessingEngine> _mockEngine;
        private readonly CorrectionOrchestrator _orchestrator;

        public CorrectionOrchestratorTests()
        {
            _mockEngine = new Mock<ITextProcessingEngine>();
            _orchestrator = new CorrectionOrchestrator(_mockEngine.Object);
        }

        [Fact]
        public async Task ProcessCorrectionAsync_ValidText_ReturnsCompletedSession()
        {
            // Arrange
            var inputText = "Je veut un café";
            var correctedText = "Je veux un café";
            _mockEngine.Setup(e => e.CorrectTextAsync(inputText))
                      .ReturnsAsync(correctedText);

            // Act
            var session = await _orchestrator.ProcessCorrectionAsync(inputText);

            // Assert
            session.Status.Should().Be(CorrectionStatus.Completed);
            session.CorrectedText.Should().Be(correctedText);
            _mockEngine.Verify(e => e.CorrectTextAsync(inputText), Times.Once);
        }

        [Fact]
        public async Task ProcessCorrectionAsync_WhenEngineFails_Throws()
        {
            // Arrange
            _mockEngine.Setup(e => e.CorrectTextAsync(It.IsAny<string>()))
                      .ThrowsAsync(new Exception("Mocked engine failure"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _orchestrator.ProcessCorrectionAsync("test")
            );
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task ProcessCorrectionAsync_InvalidText_ThrowsArgumentException(string invalidText)
        {
            // Act & Assert
            await Assert.ThrowsAsync<InvalidTextException>(() =>
                _orchestrator.ProcessCorrectionAsync(invalidText)
            );
            _mockEngine.Verify(e => e.CorrectTextAsync(It.IsAny<string>()), Times.Never);
        }
    }
}
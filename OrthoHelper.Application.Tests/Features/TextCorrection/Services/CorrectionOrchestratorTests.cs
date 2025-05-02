// OrthoHelper.Domain.Tests/Services/CorrectionOrchestratorTests.cs
using FluentAssertions;
using Moq;
using OrthoHelper.Domain.Features.TextCorrection.Ports;

using OrthoHelper.Domain.Features.TextCorrection.Exceptions;
using OrthoHelper.Domain.Features.TextCorrection.Entities;
using OrthoHelper.Domain.Features.TextCorrection.Ports.Repositories;
using OrthoHelper.Domain.Features.TextCorrection.ValueObjects;
using OrthoHelper.Application.Features.TextCorrection.Services;
namespace OrthoHelper.Domain.Tests.Features.TextCorrection.Services
{

    public class CorrectionOrchestratorTests
    {
        private readonly Mock<ITextProcessingEngine> _mockEngine;
        private readonly Mock<ILLMModelRepository> _mockModelRepository;
        private readonly CorrectionOrchestrator _orchestrator;

        public CorrectionOrchestratorTests()
        {
            _mockEngine = new Mock<ITextProcessingEngine>();
            _mockModelRepository = new Mock<ILLMModelRepository>();
            _orchestrator = new CorrectionOrchestrator(_mockEngine.Object, _mockModelRepository.Object);
        }

        [Fact]
        public async Task ProcessCorrectionAsync_ValidText_ReturnsCompletedSession()
        {
            // Arrange
            var inputText = "Je veut un café";
            var correctedText = "Je veux un café";
            var textToCorrect = new TextToCorrect(inputText);
            var modelName = new ModelName("TestModel");

            _mockEngine.Setup(e => e.CorrectTextAsync(inputText))
                       .ReturnsAsync(correctedText);

            _mockModelRepository.Setup(repo => repo.GetAvailableLLMModelsAsync())
                                .ReturnsAsync(new List<LLMModel> { new LLMModel("TestModel") });

            // Act
            var session = await _orchestrator.ProcessCorrectionAsync(textToCorrect, modelName, "TestUser");

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

            var textToCorrect = new TextToCorrect("test");
            var modelName = new ModelName("TestModel");

            // Act & Assert
            await Assert.ThrowsAsync<InvalidModelNameException>(() =>
                _orchestrator.ProcessCorrectionAsync(textToCorrect, modelName, "TestUser")
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
            {
                var textToCorrect = new TextToCorrect(invalidText);
                var modelName = new ModelName("TestModel");
                return _orchestrator.ProcessCorrectionAsync(textToCorrect, modelName, "TestUser");
            });
            _mockEngine.Verify(e => e.CorrectTextAsync(It.IsAny<string>()), Times.Never);
        }
    }
}
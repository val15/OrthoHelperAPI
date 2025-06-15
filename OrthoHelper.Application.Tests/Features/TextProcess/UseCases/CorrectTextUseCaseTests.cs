using FluentAssertions;
using Moq;
using OrthoHelper.Application.Features.TextCorrection.DTOs;
using OrthoHelper.Application.Features.TextCorrection.UseCases;
using OrthoHelper.Domain.Features.Common.Ports;
using OrthoHelper.Domain.Features.TextCorrection.Entities;
using OrthoHelper.Domain.Features.TextCorrection.Exceptions;
using OrthoHelper.Domain.Features.TextCorrection.Ports;
using OrthoHelper.Domain.Features.TextCorrection.Ports.Repositories;
using OrthoHelper.Domain.Features.TextCorrection.ValueObjects;

namespace OrthoHelper.Application.Tests.Features.TextCorrection.UseCases;

public class CorrectTextUseCaseTests
{
  
    private readonly Mock<ICurrentUserService> _mockCurrentUserService = new();
    private readonly Mock<ISessionRepository> _mockCorrectionSessionRepository = new();
    private readonly Mock<ILLMModelRepository> _mockLLMModelRepository = new();
    private readonly Mock<ICorrectorOrchestrator> _mockCorrectionOrchestrator = new();

    private readonly CorrectTextUseCase _useCase;

    public CorrectTextUseCaseTests()
    {

        _mockCurrentUserService.SetupGet(x => x.UserName).Returns("test user");
        _mockCurrentUserService.SetupGet(x => x.IsAuthenticated).Returns(true);
        _mockCurrentUserService.SetupGet(x => x.UserId).Returns(123);
        _mockLLMModelRepository.Setup(x => x.GetAvailableLLMModelsAsync())
            .ReturnsAsync(new List<LLMModel> { new LLMModel("Ollama:gemma3"), new LLMModel("Online:gemini-2.5-flash") });
        _useCase = new CorrectTextUseCase(_mockCorrectionOrchestrator.Object, 
            _mockCurrentUserService.Object, 
            _mockCorrectionSessionRepository.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidText_ReturnsOutputText()
    {
        // Arrange
        var modelName = "Ollama:gemma3";
        var input = new InputTextDto("Je veut un café", modelName);
        //_mockEngine.Setup(e => e.CorrectTextAsync(input.Text))
        //          .ReturnsAsync("Je veux un café");
        var fakeCorrectionSession = Session.Create(input.Text);
        fakeCorrectionSession.OutputText = "Je veux un café";
        fakeCorrectionSession.ModelName = modelName;
        fakeCorrectionSession.Type = Session.MessageType.Corrector;
        _mockCorrectionOrchestrator
    .Setup(o => o.ProcessAsync(It.IsAny<TextToCorrect>(), It.IsAny<ModelName>(), It.IsAny<string>()))
    .ReturnsAsync(fakeCorrectionSession);

        // Act
        var result = await _useCase.ExecuteAsync(input);

        // Assert
        input.ModelName.Should().Be(modelName);
        result.InputText.Should().Be("Je veut un café");
        result.OutputText.Should().Be("Je veux un café");
        result.ModelName.Should().Be(modelName);
        //   _mockEngine.Verify(e => e.CorrectTextAsync(input.Text), Times.Once);
    }


    [Fact]
    public async Task ExecuteAsync_WithNotFoundModel_ThrowsArgumentException()
    {
        // Arrange
        var modelName = "InvalidModel";
        var input = new InputTextDto("Je veut un café", modelName);

        _mockCorrectionOrchestrator
       .Setup(o => o.ProcessAsync(It.IsAny<TextToCorrect>(), It.IsAny<ModelName>(), It.IsAny<string>()))
       .ThrowsAsync(new InvalidModelNameException("Le modèle spécifié n'est pas valide."));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidModelNameException>(() => _useCase.ExecuteAsync(input));
        _mockCorrectionOrchestrator.Verify(o => o.ProcessAsync(
            It.IsAny<TextToCorrect>(),
            It.IsAny<ModelName>(),
            It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithEmptyText_ThrowsArgumentException()
    {
        // Arrange
        var modelName = "Ollama:gemma3";
        var input = new InputTextDto("", modelName);

        _mockCorrectionOrchestrator
            .Setup(o => o.ProcessAsync(It.IsAny<TextToCorrect>(), It.IsAny<ModelName>(), It.IsAny<string>()))
            .ThrowsAsync(new InvalidTextException("Le texte à corriger ne peut pas être vide."));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidTextException>(() => _useCase.ExecuteAsync(input));
        _mockCorrectionOrchestrator.Verify(o => o.ProcessAsync(
            It.IsAny<TextToCorrect>(),
            It.IsAny<ModelName>(),
            It.IsAny<string>()), Times.Never);
    }
}
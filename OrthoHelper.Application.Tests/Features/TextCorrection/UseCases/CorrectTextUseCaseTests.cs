using FluentAssertions;
using Moq;
using OrthoHelper.Application.Features.TextCorrection.DTOs;
using OrthoHelper.Application.Features.TextCorrection.UseCases;
using OrthoHelper.Domain.Features.Common.Ports;
using OrthoHelper.Domain.Features.TextCorrection.Exceptions;
using OrthoHelper.Domain.Features.TextCorrection.Ports;
using OrthoHelper.Domain.Features.TextCorrection.Ports.Repositories;

namespace OrthoHelper.Application.Tests.Features.TextCorrection.UseCases;

public class CorrectTextUseCaseTests
{
    private readonly Mock<ITextProcessingEngine> _mockEngine = new();
    private readonly Mock<ICurrentUserService> _mockCurrentUserService =new ();
    private readonly Mock<ICorrectionSessionRepository> _mockCorrectionSessionRepository = new();
    private readonly CorrectTextUseCase _useCase;

    public CorrectTextUseCaseTests()
    {

        _mockCurrentUserService.SetupGet(x => x.UserName).Returns("test user");
        _mockCurrentUserService.SetupGet(x => x.IsAuthenticated).Returns(true);
        _mockCurrentUserService.SetupGet(x => x.UserId).Returns(123); 
        _useCase = new CorrectTextUseCase(_mockEngine.Object, _mockCurrentUserService.Object, _mockCorrectionSessionRepository.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidText_ReturnsCorrectedText()
    {
        // Arrange
        var modelName = "Ollama:gemma3";
        var input = new CorrectTextInputDto("Je veut un café", modelName);
        _mockEngine.Setup(e => e.CorrectTextAsync(input.Text))
                  .ReturnsAsync("Je veux un café");

        // Act
        var result = await _useCase.ExecuteAsync(input);

        // Assert
        input.ModelName.Should().Be(modelName);
        result.InputText.Should().Be("Je veut un café");
        result.OutputText.Should().Be("Je veux un café");
        result.InputText.Should().Be("Je veut un café");
        _mockEngine.Verify(e => e.CorrectTextAsync(input.Text), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithEmptyText_ThrowsArgumentException()
    {
        // Arrange
        var modelName = "Ollama:gemma3";
        var input = new CorrectTextInputDto("", modelName);

        // Act & Assert
        input.ModelName.Should().Be(modelName);
        await Assert.ThrowsAsync<InvalidTextException>(() => _useCase.ExecuteAsync(input));
        _mockEngine.Verify(e => e.CorrectTextAsync(It.IsAny<string>()), Times.Never);
    }
}
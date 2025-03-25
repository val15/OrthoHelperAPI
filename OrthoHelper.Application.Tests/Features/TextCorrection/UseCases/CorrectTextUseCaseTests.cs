using FluentAssertions;
using Moq;
using OrthoHelper.Application.Features.TextCorrection.DTOs;
using OrthoHelper.Application.Features.TextCorrection.UseCases;
using OrthoHelper.Domain.Exceptions;
using OrthoHelper.Domain.Ports;
using Xunit;

namespace OrthoHelper.Application.Tests.Features.TextCorrection.UseCases;

public class CorrectTextUseCaseTests
{
    private readonly Mock<ITextProcessingEngine> _mockEngine = new();
    private readonly CorrectTextUseCase _useCase;

    public CorrectTextUseCaseTests()
    {
        _useCase = new CorrectTextUseCase(_mockEngine.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidText_ReturnsCorrectedText()
    {
        // Arrange
        var input = new CorrectTextInputDto("Je veut un café");
        _mockEngine.Setup(e => e.CorrectTextAsync(input.Text))
                  .ReturnsAsync("Je veux un café");

        // Act
        var result = await _useCase.ExecuteAsync(input);

        // Assert
        result.CorrectedText.Should().Be("Je veux un café");
        result.OriginalText.Should().Be("Je veut un café");
        _mockEngine.Verify(e => e.CorrectTextAsync(input.Text), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithEmptyText_ThrowsArgumentException()
    {
        // Arrange
        var input = new CorrectTextInputDto("");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidTextException>(() => _useCase.ExecuteAsync(input));
        _mockEngine.Verify(e => e.CorrectTextAsync(It.IsAny<string>()), Times.Never);
    }
}
using Microsoft.AspNetCore.Mvc;
using Moq;
using OrthoHelper.Api.Controllers;
using OrthoHelper.Application.Features.TextCorrection.DTOs;
using FluentAssertions;
using OrthoHelper.Application.Tests.Features.TextCorrection.UseCases;
using MediatR;
using OrthoHelper.Domain.Features.TextCorrection.Entities;
using OrthoHelper.Application.Features.TextCorrection.Queries;

namespace OrthoHelperAPI.Tests.Controllers
{
    public class TextCorrectionControllerTests
    {
        private readonly Mock<ICorrectTextUseCase> _mockUseCase = new();
        private readonly Mock<IMediator> _mockMediator = new();
        private readonly TextCorrectionController _controller;


        public TextCorrectionControllerTests()
        {
            _controller = new TextCorrectionController(_mockUseCase.Object, _mockMediator.Object);
        }

        [Fact]
        public async Task CorrectText_WithEmptyInput_ReturnsBadRequest()
        {
            // Arrange
            var input = new CorrectTextInputDto("", "Ollama:Gemma");
            _mockUseCase.Setup(uc => uc.ExecuteAsync(input))
                       .ThrowsAsync(new ArgumentException("Le texte ne peut pas être vide."));

            // Act
            var result = await _controller.CorrectText(input);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task CorrectText_WithValidInput_ReturnsOkResult()
        {
            // Arrange
            var input = new CorrectTextInputDto("Je veut un café", "Ollama:Gemma");
            var expectedOutput = new CorrectTextOutputDto
            {
                InputText = "Je veut un café",
                OutputText = "Je veux un café",
                CreatedAt = DateTime.UtcNow,
            };
                
                //(, , DateTime.UtcNow);

            _mockUseCase.Setup(uc => uc.ExecuteAsync(input))
                       .ReturnsAsync(expectedOutput);

            // Act
            var result = await _controller.CorrectText(input);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(expectedOutput);
        }


        [Fact]
        public async Task BrowseAvailableMMLModels_WhenCalled_ReturnsOkWithData()
        {
            // Arrange
            var fakeLlms = new List<LLMModelOutputDto>
        {
            new() { ModelName = "Ollama:mistral" },
            new() { ModelName = "Ollama:llama" }
        };




            _mockMediator
                .Setup(m => m.Send(It.IsAny<BrowseLLMModelsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(fakeLlms);

            // Act
            var result = await _controller.BrowseAvailableModels();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(fakeLlms);
        }

        
    }
}

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
        private readonly Mock<IProcessTextUseCase> _mockCorrectUseCase = new();
        private readonly Mock<IProcessTextUseCase> _mockTranslatUseCase = new();

        private readonly Mock<IMediator> _mockMediator = new();
        private readonly TextController _controller;


        public TextCorrectionControllerTests()
        {
            _controller = new TextController(_mockCorrectUseCase.Object, _mockTranslatUseCase.Object, _mockMediator.Object);
        }

        [Fact]
        public async Task CorrectText_WithEmptyInput_ReturnsBadRequest()
        {
            // Arrange
            var input = new InputTextDto("", "Ollama:Gemma");
            _mockCorrectUseCase.Setup(uc => uc.ExecuteAsync(input))
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
            var input = new InputTextDto("Je veut un café", "Ollama:Gemma");
            var expectedOutput = new OutputTextDto
            {
                InputText = "Je veut un café",
                OutputText = "Je veux un café",
                CreatedAt = DateTime.UtcNow,
            };

            //(, , DateTime.UtcNow);

            _mockCorrectUseCase.Setup(uc => uc.ExecuteAsync(input))
                       .ReturnsAsync(expectedOutput);

            // Act
            var result = await _controller.CorrectText(input);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(expectedOutput);
        }

        [Fact]
        public async Task TranslatText_WithValidInput_ReturnsOkResult()
        {
            // Arrange
            var input = new InputTextDto("Je veut un café", "Ollama:Gemma");
            var expectedOutput = new OutputTextDto
            {
                InputText = "Hello",
                OutputText = "Bonjour",
                CreatedAt = DateTime.UtcNow,
            };

            //(, , DateTime.UtcNow);

            _mockTranslatUseCase.Setup(uc => uc.ExecuteAsync(input))
                       .ReturnsAsync(expectedOutput);

            // Act
            var result = await _controller.TranslateText(input);

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


        [Fact]
        public async Task BrowseCorrectionSession_WhenCalled_ReturnsOkWithData()
        {
            // Arrange
            var fakeCorrectionList = new List<OutputTextDto>
        {
            new() { InputText = "Test1",OutputText="Text 1" },
            new() { InputText = "Test2",OutputText="Text 2" }

        };




            _mockMediator
                .Setup(m => m.Send(It.IsAny<BrowseSessionQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(fakeCorrectionList);

            // Act
            var result = await _controller.BrowseCorrectionSessions();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(fakeCorrectionList);
        }


        [Fact]
        public async Task DeleteAllUserCorrectionSession_WhenCalled_ReturnTrue()
        {
            // Arrange
            var fakeCorrectionList = new List<OutputTextDto>
        {
            new() { InputText = "Test1",OutputText="Text 1" },
            new() { InputText = "Test2",OutputText="Text 2" }

        };




            _mockMediator
                .Setup(m => m.Send(It.IsAny<DeleteAllUserSessionQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(fakeCorrectionList.Count);

            // Act
            var result = await _controller.DeleteAllMessages();

            // Assert
            result.Equals(2);
        }



    }
}

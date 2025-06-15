using Microsoft.AspNetCore.Mvc;
using Moq;
using OrthoHelper.Api.Controllers;
using OrthoHelper.Application.Features.TextCorrection.DTOs;
using FluentAssertions;
using MediatR;
using OrthoHelper.Application.Features.TextCorrection.Queries;
using OrthoHelper.Application.Features.TextTranslation;
using OrthoHelper.Application.Features.TextProcess.UseCases;

namespace OrthoHelperAPI.Tests.Controllers
{
    /// <summary>
    /// Contains unit tests for the TextController, verifying its interaction with use cases and MediatR.
    /// </summary>
    public class TextControllerTests
    {
        // Mocks for dependencies
        private readonly Mock<ICorrectTextUseCase> _mockCorrectUseCase = new();
        private readonly Mock<ITranslateTextUseCase> _mockTranslateUseCase = new();
        private readonly Mock<ITranslateHtmlFileUseCase> _mockTranslateHtmlFileUseCase = new();
        private readonly Mock<IMediator> _mockMediator = new();

        // The controller instance under test
        private readonly TextController _controller;

        public TextControllerTests()
        {
            // Initialize the controller with its mocked dependencies.
            _controller = new TextController(
                _mockCorrectUseCase.Object,
                _mockTranslateUseCase.Object,
                _mockTranslateHtmlFileUseCase.Object,
                _mockMediator.Object);
        }

        /// <summary>
        /// Function: CorrectText (POST)
        /// Scenario: When the input text is empty.
        /// Expected Result: Should return a BadRequestObjectResult.
        /// </summary>
        [Fact]
        public async Task CorrectText_WithEmptyInput_ShouldReturnBadRequest()
        {
            // ARRANGE
            // 1. Define an input with empty text.
            var input = new InputTextDto("", "Ollama:Gemma");

            // 2. Configure the mock use case to throw an exception, simulating a validation failure.
            _mockCorrectUseCase.Setup(uc => uc.ExecuteAsync(input))
                               .ThrowsAsync(new ArgumentException("Text cannot be empty."));

            // ACT
            // Call the controller action.
            var result = await _controller.CorrectText(input);

            // ASSERT
            // Verify that the controller correctly handled the exception and returned a 400 Bad Request.
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        /// <summary>
        /// Function: CorrectText (POST)
        /// Scenario: When the input is valid.
        /// Expected Result: Should return an OkObjectResult with the corrected text.
        /// </summary>
        [Fact]
        public async Task CorrectText_WithValidInput_ShouldReturnOkWithCorrectedText()
        {
            // ARRANGE
            // 1. Define valid input and the expected successful output.
            var input = new InputTextDto("I wants a coffee.", "Ollama:Gemma");
            var expectedOutput = new OutputTextDto
            {
                InputText = "I wants a coffee.",
                OutputText = "I want a coffee.",
                CreatedAt = DateTime.UtcNow,
            };

            // 2. Configure the mock use case to return the successful output.
            _mockCorrectUseCase.Setup(uc => uc.ExecuteAsync(input))
                               .ReturnsAsync(expectedOutput);

            // ACT
            var result = await _controller.CorrectText(input);

            // ASSERT
            // 1. Verify the result is an HTTP 200 OK.
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;

            // 2. Verify the content of the result matches the expected output.
            okResult.Value.Should().BeEquivalentTo(expectedOutput);
        }

        /// <summary>
        /// Function: TranslateText (POST)
        /// Scenario: When the input is valid.
        /// Expected Result: Should return an OkObjectResult with the translated text.
        /// </summary>
        [Fact]
        public async Task TranslateText_WithValidInput_ShouldReturnOkWithTranslatedText()
        {
            // ARRANGE
            var input = new InputTextDto("Hello", "Ollama:Gemma");
            var expectedOutput = new OutputTextDto
            {
                InputText = "Hello",
                OutputText = "Bonjour", // "Hello" in French
                CreatedAt = DateTime.UtcNow,
            };

            _mockTranslateUseCase.Setup(uc => uc.ExecuteAsync(input))
                                 .ReturnsAsync(expectedOutput);

            // ACT
            var result = await _controller.TranslateText(input);

            // ASSERT
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(expectedOutput);
        }

        /// <summary>
        /// Function: BrowseAvailableModels (GET)
        /// Scenario: When the endpoint is called.
        /// Expected Result: Should return an OkObjectResult with a list of LLM models.
        /// </summary>
        [Fact]
        public async Task BrowseAvailableModels_WhenCalled_ShouldReturnOkWithListOfModels()
        {
            // ARRANGE
            // 1. Create a fake list of models to be returned by MediatR.
            var fakeLlms = new List<LLMModelOutputDto>
        {
            new() { ModelName = "Ollama:mistral" },
            new() { ModelName = "Ollama:llama" }
        };

            // 2. Configure MediatR to return the fake list when the corresponding query is sent.
            _mockMediator
                .Setup(m => m.Send(It.IsAny<BrowseLLMModelsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(fakeLlms);

            // ACT
            var result = await _controller.BrowseAvailableModels();

            // ASSERT
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(fakeLlms);
        }

        /// <summary>
        /// Function: DeleteAllMessages (DELETE)
        /// Scenario: When the endpoint is called to delete all sessions.
        /// Expected Result: Should return an OkObjectResult with the count of deleted items.
        /// </summary>
        [Fact]
        public async Task DeleteAllMessages_WhenCalled_ShouldReturnOkWithDeletedCount()
        {
            // ARRANGE
            // 1. Define the expected count of deleted items.
            var deletedCount = 2;

            // 2. Configure MediatR to return the count when the delete command is sent.
            _mockMediator
                .Setup(m => m.Send(It.IsAny<DeleteAllUserSessionQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(deletedCount);

            // ACT
            var result = await _controller.DeleteAllMessages();

            // ASSERT
            // **CRITICAL FIX**: Verify the result is an OkObjectResult and check its Value property.
            // The original `result.Equals(2)` does not perform a valid assertion.
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(deletedCount);
        }
    }
}

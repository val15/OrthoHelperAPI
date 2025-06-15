using Moq;
using OrthoHelper.Application.Features.TextTranslation.DTOs;
using OrthoHelper.Application.Features.TextTranslation;
using OrthoHelper.Domain.Features.TextCorrection.Ports;
using OrthoHelper.Domain.Features.TextTranslation.Ports;
using OrthoHelper.Domain.Features.TextTranslation.Entities;

namespace OrthoHelper.Application.Tests.Features.TextTranslation.UserCases
{
    /// <summary>
    /// Contains unit tests for the TranslateHtmlFileUseCase, covering successful translations
    /// and various failure scenarios.
    /// </summary>
    public class TranslateHtmlFileUseCaseTests
    {
        // Mocks for the use case's dependencies
        private readonly Mock<IHtmlParser> _mockHtmlParser;
        private readonly Mock<ITextProcessingEngine> _mockTextProcessingEngine;

        // The instance of the class under test
        private readonly TranslateHtmlFileUseCase _useCase;

        /// <summary>
        /// The constructor initializes the mocks and the use case instance for all tests in this class.
        /// </summary>
        public TranslateHtmlFileUseCaseTests()
        {
            _mockHtmlParser = new Mock<IHtmlParser>();
            _mockTextProcessingEngine = new Mock<ITextProcessingEngine>();
            _useCase = new TranslateHtmlFileUseCase(_mockHtmlParser.Object, _mockTextProcessingEngine.Object);
        }

        /// <summary>
        /// Function: ExecuteAsync
        /// Scenario: With a valid HTML file and successful processing.
        /// Expected Result: Should return a result object with the path to the translated file.
        /// </summary>
        [Fact]
        public async Task ExecuteAsync_WithValidFile_ReturnsSuccessfulTranslationResult()
        {
            // ARRANGE
            // 1. Define the input for the use case, including file path and model name.
            var input = new HtmlTranslationInputDto
            {
                HtmlFilePath = "TestFiles/sample.html",
                ModelName = "Ollama:Gemma"
            };

            // 2. Define the data that mocks will interact with.
            var translatableText = "Hello, world!\nThis is a test.";
            var translatedHtmlPath = "TestFiles/translated.html";

            // 3. Configure the HTML parser mock to return the extracted text from the file.
            _mockHtmlParser
                .Setup(p => p.ExtractTranslatableTextAsync(input.HtmlFilePath))
                .ReturnsAsync(translatableText);

            // 4. Configure the engine to simulate the translation of each individual sentence.
            _mockTextProcessingEngine
                .Setup(e => e.ProcessTextAsync("Hello, world!"))
                .ReturnsAsync("Bonjour, le monde!"); // "Hello, world!" in French
            _mockTextProcessingEngine
                .Setup(e => e.ProcessTextAsync("This is a test."))
                .ReturnsAsync("Ceci est un test."); // "This is a test." in French

            // 5. Configure the engine to return its model name when asked.
            _mockTextProcessingEngine
                .Setup(e => e.GetModelName())
                .Returns(input.ModelName);

            // 6. Configure the HTML parser to simulate replacing text and returning the new file path.
            _mockHtmlParser
                .Setup(p => p.ReplaceTranslatedTextAsync(
                    input.HtmlFilePath,
                    It.IsAny<Dictionary<string, string>>(), // We accept any dictionary for this setup
                    input.ModelName,
                    null))
                .ReturnsAsync(translatedHtmlPath);

            // ACT
            // Execute the use case with the defined input.
            var result = await _useCase.ExecuteAsync(input);

            // ASSERT
            // 1. Check that the result object is valid and contains the expected data.
            Assert.NotNull(result);
            Assert.Equal(input.HtmlFilePath, result.OriginalHtmlPath);
            Assert.Equal(translatedHtmlPath, result.TranslatedHtmlPath);
            Assert.True(result.CreatedAt <= DateTime.UtcNow);

            // 2. Verify that all key methods on our mocks were called exactly once.
            _mockHtmlParser.Verify(p => p.ExtractTranslatableTextAsync(input.HtmlFilePath), Times.Once);
            _mockHtmlParser.Verify(p => p.ReplaceTranslatedTextAsync(
                input.HtmlFilePath,
                It.IsAny<Dictionary<string, string>>(),
                input.ModelName,
                null), Times.Once);
            _mockTextProcessingEngine.Verify(e => e.ProcessTextAsync("Hello, world!"), Times.Once);
            _mockTextProcessingEngine.Verify(e => e.ProcessTextAsync("This is a test."), Times.Once);
        }

        /// <summary>
        /// Function: ExecuteAsync
        /// Scenario: When the provided HTML file path is invalid (e.g., empty).
        /// Expected Result: Should throw an InvalidHtmlFileException.
        /// </summary>
        [Fact]
        public async Task ExecuteAsync_WithInvalidFilePath_ThrowsInvalidHtmlFileException()
        {
            // ARRANGE
            // 1. Define an input with an invalid (empty) file path.
            var input = new HtmlTranslationInputDto
            {
                HtmlFilePath = "",
                ModelName = "Ollama:Gemma"
            };

            // 2. Configure the mock parser to throw the expected exception when it receives an invalid path.
            _mockHtmlParser
                .Setup(p => p.ExtractTranslatableTextAsync(input.HtmlFilePath))
                .ThrowsAsync(new InvalidHtmlFileException("Invalid path"));

            // ACT & ASSERT
            // Verify that executing the use case throws the specific exception we expect.
            await Assert.ThrowsAsync<InvalidHtmlFileException>(() => _useCase.ExecuteAsync(input));
        }

        /// <summary>
        /// Function: ExecuteAsync
        /// Scenario: When the text processing engine encounters an internal error.
        /// Expected Result: Should throw a general Exception.
        /// </summary>
        [Fact]
        public async Task ExecuteAsync_WhenProcessingEngineFails_ThrowsException()
        {
            // ARRANGE
            // 1. Define a valid input.
            var input = new HtmlTranslationInputDto
            {
                HtmlFilePath = "TestFiles/sample.html",
                ModelName = "Ollama:Gemma"
            };

            // 2. Configure the parser mock to successfully extract text.
            _mockHtmlParser
                .Setup(p => p.ExtractTranslatableTextAsync(input.HtmlFilePath))
                .ReturnsAsync("Some text to translate.");

            // 3. Configure the engine mock to simulate a failure during its processing.
            _mockTextProcessingEngine
                .Setup(e => e.ProcessTextAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Text processing failed."));

            // ACT & ASSERT
            // Verify that the exception from the engine is correctly passed up by the use case.
            var ex = await Assert.ThrowsAsync<Exception>(() => _useCase.ExecuteAsync(input));
            Assert.Equal("Text processing failed.", ex.Message);
        }
    }
}

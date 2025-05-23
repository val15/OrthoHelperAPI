﻿using Moq;
using OrthoHelper.Application.Features.TextTranslation.DTOs;
using OrthoHelper.Application.Features.TextTranslation;
using OrthoHelper.Domain.Features.TextCorrection.Ports;
using OrthoHelper.Domain.Features.TextTranslation.Ports;
using OrthoHelper.Domain.Features.TextTranslation.Entities;

namespace OrthoHelper.Application.Tests.Features.TextTranslation.UserCases
{
    public class TranslateHtmlFileUseCaseTests
    {
        private readonly Mock<IHtmlParser> _mockHtmlParser;
        private readonly Mock<ITextProcessingEngine> _mockTextProcessingEngine;
        private readonly TranslateHtmlFileUseCase _useCase;

        public TranslateHtmlFileUseCaseTests()
        {
            _mockHtmlParser = new Mock<IHtmlParser>();
            _mockTextProcessingEngine = new Mock<ITextProcessingEngine>();
            _useCase = new TranslateHtmlFileUseCase(_mockHtmlParser.Object, _mockTextProcessingEngine.Object);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldTranslateHtmlFileSuccessfully()
        {
            // Arrange
            var input = new HtmlTranslationInputDto
            {
                HtmlFilePath = "TestFiles/sample.html",
                ModelName = "Ollama:Gemma"
            };

            var translatableText = "Hello, world!\nThis is a test.";
            var translations = new Dictionary<string, string>
        {
            { "Hello, world!", "Bonjour, le monde!" },
            { "This is a test.", "Ceci est un test." }
        };
            var translatedHtmlPath = "TestFiles/translated.html";

            _mockHtmlParser
                .Setup(p => p.ExtractTranslatableTextAsync(input.HtmlFilePath))
                .ReturnsAsync(translatableText);

            // Simule la traduction de chaque texte
            _mockTextProcessingEngine
                .Setup(e => e.ProcessTextAsync("Hello, world!"))
                .ReturnsAsync("Bonjour, le monde!");
            _mockTextProcessingEngine
                .Setup(e => e.ProcessTextAsync("This is a test."))
                .ReturnsAsync("Ceci est un test.");

            _mockTextProcessingEngine
                .Setup(e => e.GetModelName())
                .Returns(input.ModelName);

            _mockHtmlParser
                .Setup(p => p.ReplaceTranslatedTextAsync(
                    input.HtmlFilePath,
                    It.Is<Dictionary<string, string>>(d =>
                        d.ContainsKey("Hello, world!") && d.ContainsKey("This is a test.")),
                    input.ModelName,
                    null))
                .ReturnsAsync(translatedHtmlPath);

            // Act
            var result = await _useCase.ExecuteAsync(input);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(input.HtmlFilePath, result.OriginalHtmlPath);
            Assert.Equal(translatedHtmlPath, result.TranslatedHtmlPath);
            Assert.True(result.CreatedAt <= DateTime.UtcNow);

            _mockHtmlParser.Verify(p => p.ExtractTranslatableTextAsync(input.HtmlFilePath), Times.Once);
            _mockHtmlParser.Verify(p => p.ReplaceTranslatedTextAsync(
                input.HtmlFilePath,
                It.IsAny<Dictionary<string, string>>(),
                input.ModelName,
                null), Times.Once);
            _mockTextProcessingEngine.Verify(e => e.ProcessTextAsync("Hello, world!"), Times.Once);
            _mockTextProcessingEngine.Verify(e => e.ProcessTextAsync("This is a test."), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldThrowException_WhenHtmlFilePathIsInvalid()
        {
            // Arrange
            var input = new HtmlTranslationInputDto
            {
                HtmlFilePath = "",
                ModelName = "Ollama:Gemma"
            };

            // Simule une exception pour un chemin invalide
            _mockHtmlParser
                .Setup(p => p.ExtractTranslatableTextAsync(input.HtmlFilePath))
                .ThrowsAsync(new InvalidHtmlFileException("Invalid path"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidHtmlFileException>(() => _useCase.ExecuteAsync(input));
        }

        [Fact]
        public async Task ExecuteAsync_ShouldThrowException_WhenTextProcessingFails()
        {
            // Arrange
            var input = new HtmlTranslationInputDto
            {
                HtmlFilePath = "TestFiles/sample.html",
                ModelName = "Ollama:Gemma"
            };

            var translatableText = "Hello, world!\nThis is a test.";

            _mockHtmlParser
                .Setup(p => p.ExtractTranslatableTextAsync(input.HtmlFilePath))
                .ReturnsAsync(translatableText);

            // Simule une erreur lors de la traduction
            _mockTextProcessingEngine
                .Setup(e => e.ProcessTextAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Text processing failed."));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _useCase.ExecuteAsync(input));
            Assert.Equal("Text processing failed.", ex.Message);
        }
    }
}

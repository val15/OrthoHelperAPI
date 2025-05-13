using Microsoft.Extensions.Logging;
using Moq;
using OrthoHelper.Domain.Features.Common.Ports;
using OrthoHelper.Domain.Features.TextCorrection.Ports;
using OrthoHelper.Domain.Features.TextCorrection.Ports.Repositories;
using OrthoHelper.Domain.Features.TextTranslation.Ports;
using OrthoHelper.Infrastructure.Features.TextProcessing;
using OrthoHelper.Infrastructure.Features.TextTranslation;
using OrthoHelper.Shared.Utilities;

namespace OrthoHelper.Integration.Tests
{
    public class HtmlTranslationIntegrationTests
    {
        private readonly HttpClient _client;
        private readonly Mock<ISessionRepository> _mockRepository;
        private readonly Mock<ICurrentUserService> _mockCurrentUserService;
        private readonly Mock<ILogger<OrthoEngineTranslator>> _mockLogger;
        private readonly IHtmlParser _htmlParser;
        private readonly ITextProcessingEngine _textProcessingEngine;

        public HtmlTranslationIntegrationTests()
        {
            _client = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(15),
                BaseAddress = new Uri("http://localhost:11434")
            };

            // Mock ISessionRepository
            _mockRepository = new Mock<ISessionRepository>();

            // Mock ICurrentUserService
            _mockCurrentUserService = new Mock<ICurrentUserService>();
            _mockCurrentUserService.SetupGet(x => x.UserName).Returns("testuser");
            _mockCurrentUserService.SetupGet(x => x.IsAuthenticated).Returns(true);

            // Mock ILogger
            _mockLogger = new Mock<ILogger<OrthoEngineTranslator>>();

            // Utiliser les implémentations réelles pour IHtmlParser et ITextProcessingEngine
            _htmlParser = new HtmlParser();
            var orthoEngineTranslator = new OrthoEngineTranslator(
                _client, // HttpClient réel
                _mockRepository.Object,
                _mockCurrentUserService.Object,
                _mockLogger.Object
            );


            _textProcessingEngine = new OrthoEngineAdapter(orthoEngineTranslator
                );
        }

        [Fact]
        public async Task TranslateHtmlFile_gemini20_ShouldSampleTranslateSuccessfully()
        {
            _textProcessingEngine.SetModelName("Online:gemini-2.0-flash");
            // orthoEngineTranslator.ModelName = "Ollama:gemma3";

            // Arrange
            var htmlFilePath = "TestFiles/.html"; // Assurez-vous que ce fichier existe
          //  var expectedTranslatedHtmlPath = "TestFiles/sampleGemini20Translated.html";

            // Act
            // Étape 1 : Extraire le texte traductible
            var translatableText = await _htmlParser.ExtractTranslatableTextAsync(htmlFilePath);

            // Étape 2 : Traduire chaque texte
            var translations = await TranslationHelper.TranslateTextsAsync(translatableText, _textProcessingEngine);

            // Étape 3 : Remplacer les textes traduits dans le fichier HTML
            var translatedHtmlPath = await _htmlParser.ReplaceTranslatedTextAsync(htmlFilePath, translations,_textProcessingEngine.GetModelName());

            // Assert
            Assert.True(File.Exists(translatedHtmlPath));
            var translatedHtmlContent = File.ReadAllText(translatedHtmlPath);
            Assert.Contains("Bonjour".ToLower(), translatedHtmlContent.ToLower());
            Assert.Contains("Ceci".ToLower(), translatedHtmlContent.ToLower());
        }

        [Fact]
        public async Task TranslateHtmlFile_gemma3_ShouldSampleTranslateSuccessfully()
        {
            _textProcessingEngine.SetModelName("Ollama:gemma3");
            // orthoEngineTranslator.ModelName = "Ollama:gemma3";

            // Arrange
            var htmlFilePath = "TestFiles/sample.html"; // Assurez-vous que ce fichier existe
            var expectedTranslatedHtmlPath = "TestFiles/sampleGemma3Translated.html";

            // Act
            // Étape 1 : Extraire le texte traductible
            var translatableText = await _htmlParser.ExtractTranslatableTextAsync(htmlFilePath);

            // Étape 2 : Traduire chaque texte
            var translations = await TranslationHelper.TranslateTextsAsync(translatableText, _textProcessingEngine);

            // Étape 3 : Remplacer les textes traduits dans le fichier HTML
            var translatedHtmlPath = await _htmlParser.ReplaceTranslatedTextAsync(htmlFilePath, translations,expectedTranslatedHtmlPath);

            // Assert
            Assert.True(File.Exists(translatedHtmlPath));
            var translatedHtmlContent = File.ReadAllText(translatedHtmlPath);
            Assert.Contains("Bonjour".ToLower(), translatedHtmlContent.ToLower());
            Assert.Contains("Ceci".ToLower(), translatedHtmlContent.ToLower());
        }


        [Fact]
        public async Task TranslateHtmlFile_gemma3_lotterylettersConverted_ShouldTranslateSuccessfully()
        {
            _textProcessingEngine.SetModelName("Ollama:gemma3");
            // orthoEngineTranslator.ModelName = "Ollama:gemma3";

            // Arrange
            var htmlFilePath = "TestFiles/lotteryletters-converted.html"; // Assurez-vous que ce fichier existe
           
            // Act
            // Étape 1 : Extraire le texte traductible
            var translatableText = await _htmlParser.ExtractTranslatableTextAsync(htmlFilePath);

            // Étape 2 : Traduire chaque texte
            var translations = await TranslationHelper.TranslateTextsAsync(translatableText, _textProcessingEngine);

            // Étape 3 : Remplacer les textes traduits dans le fichier HTML
            var translatedHtmlPath = await _htmlParser.ReplaceTranslatedTextAsync(htmlFilePath, translations, _textProcessingEngine.GetModelName());

            // Assert
            Assert.True(File.Exists(translatedHtmlPath));
            var translatedHtmlContent = File.ReadAllText(translatedHtmlPath);
            Assert.Contains("Bonjour".ToLower(), translatedHtmlContent.ToLower());
            Assert.Contains("Ceci".ToLower(), translatedHtmlContent.ToLower());
        }

        [Fact]
        public async Task TranslateHtmlFile_gemini_lotterylettersConverted_ShouldTranslateSuccessfully()
        {
            _textProcessingEngine.SetModelName("Online:gemini-2.0-flash");

            // Arrange
            var htmlFilePath = "TestFiles/lotteryletters-converted.html"; // Assurez-vous que ce fichier existe

            // Act
            // Étape 1 : Extraire le texte traductible
            var translatableText = await _htmlParser.ExtractTranslatableTextAsync(htmlFilePath);

            // Étape 2 : Traduire chaque texte
            var translations = await TranslationHelper.TranslateTextsAsync(translatableText, _textProcessingEngine);


            // Étape 3 : Remplacer les textes traduits dans le fichier HTML
            var translatedHtmlPath = await _htmlParser.ReplaceTranslatedTextAsync(htmlFilePath, translations,_textProcessingEngine.GetModelName());

            // Assert
            Assert.True(File.Exists(translatedHtmlPath));
           
        }


        [Fact]
        public async Task TranslateHtmlFile_gemma3_ShouldSmallTranslateSuccessfully()
        {
            _textProcessingEngine.SetModelName("Ollama:gemma3");
            // orthoEngineTranslator.ModelName = "Ollama:gemma3";

            // Arrange
            var htmlFilePath = "TestFiles/small.html"; // Assurez-vous que ce fichier existe
            var expectedTranslatedHtmlPath = "TestFiles/smallGemma3Translated.html";

            // Act
            // Étape 1 : Extraire le texte traductible
            var translatableText = await _htmlParser.ExtractTranslatableTextAsync(htmlFilePath);

            // Étape 2 : Traduire chaque texte
            var translations = await TranslationHelper.TranslateTextsAsync(translatableText, _textProcessingEngine);

            // Étape 3 : Remplacer les textes traduits dans le fichier HTML
            var translatedHtmlPath = await _htmlParser.ReplaceTranslatedTextAsync(htmlFilePath, translations, expectedTranslatedHtmlPath);

            // Assert
            Assert.True(File.Exists(translatedHtmlPath));
            var translatedHtmlContent = File.ReadAllText(translatedHtmlPath);
        }

        [Fact]
        public async Task TranslateHtmlFile_gemini20_ShouldSmallTranslateSuccessfully()
        {
            _textProcessingEngine.SetModelName("Online:gemini-2.0-flash");
            // orthoEngineTranslator.ModelName = "Ollama:gemma3";

            // Arrange
            var htmlFilePath = "TestFiles/small.html"; // Assurez-vous que ce fichier existe
            var expectedTranslatedHtmlPath = "TestFiles/smallGemini20Translated.html";

            // Act
            // Étape 1 : Extraire le texte traductible
            var translatableText = await _htmlParser.ExtractTranslatableTextAsync(htmlFilePath);

            // Étape 2 : Traduire chaque texte
            var translations = await TranslationHelper.TranslateTextsAsync(translatableText, _textProcessingEngine);

            // Étape 3 : Remplacer les textes traduits dans le fichier HTML
            var translatedHtmlPath = await _htmlParser.ReplaceTranslatedTextAsync(htmlFilePath, translations, expectedTranslatedHtmlPath);

            // Assert
            Assert.True(File.Exists(translatedHtmlPath));
            var translatedHtmlContent = File.ReadAllText(translatedHtmlPath);
        }


       
      

    }
}

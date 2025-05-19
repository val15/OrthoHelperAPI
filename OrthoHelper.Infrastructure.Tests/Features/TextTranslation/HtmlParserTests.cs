using OrthoHelper.Infrastructure.Features.TextTranslation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrthoHelper.Infrastructure.Tests.Features.TextTranslation
{
    public class HtmlParserTests
    {
        private readonly HtmlParser _htmlParser;

        public HtmlParserTests()
        {
            _htmlParser = new HtmlParser();
        }

        [Fact]
        public async Task ExtractTranslatableTextAsync_ShouldExtractTextFromHtml()
        {
            // Arrange
            var htmlFilePath = "TestFiles/sample.html"; // Assurez-vous que ce fichier existe dans le dossier de test
            var expectedTexts = new List<string>
            {
                "Test HTML",
                "Hello, world!",
                "This is a test."
            };

            // Act
            var result = await _htmlParser.ExtractTranslatableTextAsync(htmlFilePath);

            // Assert
            Assert.NotNull(result);
            var extractedTexts = result.Split('\n');
            Assert.Equal(expectedTexts.Count, extractedTexts.Length);
            Assert.Equal(expectedTexts, extractedTexts);
        }

        [Fact]
        public async Task ReplaceTranslatedTextAsync_ShouldReplaceTextInHtml()
        {
            // Arrange
            var htmlFilePath = "TestFiles/sample.html"; // Assurez-vous que ce fichier existe dans le dossier de test
            var translations = new Dictionary<string, string>
            {
                { "Hello, world!", "Bonjour, le monde!" },
                { "This is a test.", "Ceci est un test." }
            };
            var expectedTranslatedHtml =
 @"<!DOCTYPE html>
<html>
<head>
    <title>Test HTML</title>
</head>
<body>
    <p>Bonjour, le monde!</p>
    <p>Ceci est un test.</p>
</body>
</html>";

            // Act
            var translatedHtmlPath = await _htmlParser.ReplaceTranslatedTextAsync(htmlFilePath, translations,"TestModel");

            // Assert
            Assert.True(File.Exists(translatedHtmlPath));
            var translatedHtml = File.ReadAllText(translatedHtmlPath);
            Assert.Equal(expectedTranslatedHtml.Trim().Replace("\r\n", "\n"), translatedHtml.Trim().Replace("\r\n", "\n"));
        }
    }
}

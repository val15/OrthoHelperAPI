using FluentAssertions;
using OrthoHelper.Infrastructure.Features.TextTranslation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrthoHelper.Infrastructure.Tests.Features.TextTranslation
{
    /// <summary>
    /// Contains unit tests for the HtmlParser class.
    /// Note: These tests interact with the file system and depend on files located in the "TestFiles" directory.
    /// </summary>
    public class HtmlParserTests : IDisposable
    {
        private readonly HtmlParser _htmlParser;
        private string? _generatedFilePath; // To hold the path of any file created during a test for cleanup.

        public HtmlParserTests()
        {
            _htmlParser = new HtmlParser();
        }

        /// <summary>
        /// Function: ExtractTranslatableTextAsync
        /// Scenario: When provided with a valid HTML file.
        /// Expected Result: Should extract all translatable text nodes into a single, newline-separated string.
        /// </summary>
        [Fact]
        public async Task ExtractTranslatableTextAsync_WithValidHtmlFile_ShouldReturnNewlineSeparatedText()
        {
            // ARRANGE
            // 1. Define the path to the source test file.
            var htmlFilePath = "TestFiles/sample.html";

            // 2. Define the exact text content we expect to be extracted, in order.
            var expectedTexts = new[]
            {
            "Test HTML",
            "Hello, world!",
            "This is a test."
        };

            // ACT
            // Call the method to extract text.
            var resultString = await _htmlParser.ExtractTranslatableTextAsync(htmlFilePath);

            // ASSERT
            // 1. Verify the result is not null.
            resultString.Should().NotBeNull();

            // 2. Split the resulting string into individual lines for comparison.
            var extractedTexts = resultString.Split('\n');

            // 3. Verify that the extracted text lines are equivalent to the expected text lines.
            extractedTexts.Should().BeEquivalentTo(expectedTexts, options => options.WithStrictOrdering());
        }

        /// <summary>
        /// Function: ReplaceTranslatedTextAsync
        /// Scenario: When provided with a source HTML file and a dictionary of translations.
        /// Expected Result: Should create a new HTML file with the original text replaced by the translations.
        /// </summary>
        [Fact]
        public async Task ReplaceTranslatedTextAsync_WithValidTranslations_ShouldCreateFileWithReplacedText()
        {
            // ARRANGE
            // 1. Define the path to the source test file.
            var htmlFilePath = "TestFiles/sample.html";

            // 2. Define the translations to be applied.
            var translations = new Dictionary<string, string>
        {
            { "Hello, world!", "Bonjour, le monde!" }, // "Hello, world!" in French
            { "This is a test.", "Ceci est un test." }   // "This is a test." in French
        };

            // 3. Define the expected content of the newly generated HTML file as a string literal.
            var expectedHtmlContent =
    @"<!DOCTYPE html>
<html>
<head>
    <title>Test HTML</title>
    <style>
        body {
            font-family: Arial;
        }
    </style>
</head>
<body>
    <p>Bonjour, le monde!</p>
    <p>Ceci est un test.</p>
    <script>
        console.log(""This is a script."");
    </script>
</body>
</html>";

            // ACT
            // Call the method to perform the replacement. Store the path of the generated file for cleanup.
            _generatedFilePath = await _htmlParser.ReplaceTranslatedTextAsync(htmlFilePath, translations, "TestModel");

            // Read the content of the newly created file.
            var actualHtmlContent = await File.ReadAllTextAsync(_generatedFilePath);

            // ASSERT
            // 1. Verify that the translated file was actually created.
            File.Exists(_generatedFilePath).Should().BeTrue();

            // 2. Normalize line endings for both strings to ensure the comparison is consistent across different OS.
            var normalizedExpected = expectedHtmlContent.Trim().Replace("\r\n", "\n");
            var normalizedActual = actualHtmlContent.Trim().Replace("\r\n", "\n");

            // 3. Verify that the content of the new file is exactly what we expect.
            normalizedActual.Should().Be(normalizedExpected);
        }

        /// <summary>
        /// Cleans up any files generated during the tests to maintain a clean test environment.
        /// This method is called automatically by the test runner after all tests in the class have finished.
        /// </summary>
        public void Dispose()
        {
            if (!string.IsNullOrEmpty(_generatedFilePath) && File.Exists(_generatedFilePath))
            {
                File.Delete(_generatedFilePath);
            }
            GC.SuppressFinalize(this);
        }
    }
}

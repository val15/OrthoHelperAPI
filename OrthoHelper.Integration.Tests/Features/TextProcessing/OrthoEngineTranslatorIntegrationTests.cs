using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OrthoHelper.Domain.Features.Common.Ports;
using OrthoHelper.Domain.Features.TextCorrection.Ports.Repositories;
using OrthoHelper.Infrastructure.Features.TextProcessing;
using Xunit.Abstractions;

namespace OrthoHelper.Integration.Tests.Features.TextProcessing
{


    /// <summary>
    /// Contains INTEGRATION tests for the OrthoEngineTranslator.
    /// <strong>IMPORTANT:</strong> These tests require a running external AI service (Ollama or an online provider)
    /// and will make real API calls.
    /// </summary>
    public class OrthoEngineTranslatorIntegrationTests
    {
        // Dependencies
        private readonly HttpClient _client;
        private readonly Mock<ISessionRepository> _mockSessionRepo;
        private readonly Mock<ICurrentUserService> _mockCurrentUser;
        private readonly Mock<ILogger<OrthoEngineTranslator>> _mockLogger;
        private readonly ITestOutputHelper _output;

        // Define reusable test inputs to avoid magic strings
        private const string ShortTextInput = "Hello";
        private const string LongTextInput = "Friend’s note was among the first of the torrent of letters that arrived at The New Yorker in the wake of “The Lottery”..."; // Truncated for brevity

        public OrthoEngineTranslatorIntegrationTests(ITestOutputHelper output)
        {
            _output = output;
            _mockSessionRepo = new Mock<ISessionRepository>();
            _mockCurrentUser = new Mock<ICurrentUserService>();
            _mockLogger = new Mock<ILogger<OrthoEngineTranslator>>();
            _client = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(15),
                BaseAddress = new Uri("http://localhost:11434")
            };
        }

        /// <summary>
        /// <strong>Function:</strong> ProcessTextAsync
        /// <strong>Scenario:</strong> When translating text with various models and input lengths.
        /// <strong>Expected Result:</strong> Should return a successful translation without processing errors.
        /// </summary>
        [Theory]
      //  [InlineData("Online:gemini-2.5-flash", ShortTextInput)]
        [InlineData("Online:gemini-2.0-flash", ShortTextInput)]
        [InlineData("Ollama:gemma3", ShortTextInput)]
        [InlineData("Ollama:gemma3", LongTextInput)]
        public async Task ProcessTextAsync_WithVariousInputsAndModels_ShouldReturnSuccessfulTranslation(string modelName, string inputText)
        {
            // ARRANGE
            // 1. Create an instance of the engine to be tested.
            var engine = new OrthoEngineTranslator(_client, _mockSessionRepo.Object, _mockCurrentUser.Object, _mockLogger.Object)
            {
                ModelName = modelName
            };

            // ACT
            // Call the translation method, which will make a real API call.
            var result = await engine.ProcessTextAsync(inputText);

            // ASSERT
            // 1. Log the output for manual verification during test runs.
            _output.WriteLine($"--- Test for model: {modelName} ---");
            _output.WriteLine($"Input ({inputText.Length} chars): {inputText.Substring(0, Math.Min(inputText.Length, 70))}...");
            _output.WriteLine($"Output ({result.Length} chars): {result}");

            // 2. Verify that the result does not contain a known error message.
            result.Should().NotContain("Error during processing", "the translation should succeed.");

            // 3. Verify the engine's model name was correctly used.
            engine.ModelName.Should().Be(modelName);
        }
    }
}

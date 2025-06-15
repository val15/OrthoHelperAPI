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
    /// Contains INTEGRATION tests for the OrthoEngineCorrector.
    /// IMPORTANT: These tests require a running Ollama/Online service and will make real API calls.
    /// </summary>
    public class OrthoEngineCorrectorIntegrationTests
    {
        private readonly HttpClient _client;
        private readonly Mock<ISessionRepository> _mockSessionRepo;
        private readonly Mock<ICurrentUserService> _mockCurrentUser;
        private readonly Mock<ILogger<OrthoEngineCorrector>> _mockLogger;
        private readonly ITestOutputHelper _output;

        private const string ShortTextInput = "Salu Henitsoa, je viens vert toi a la demande de Hafalina pour donné mon coup de main sur l'application novyparking que tu develope";
        private const string LongTextInput = "1 : Gestion de coupur internet..."; // Shortened for brevity

        public OrthoEngineCorrectorIntegrationTests(ITestOutputHelper output)
        {
            _output = output;
            _mockSessionRepo = new Mock<ISessionRepository>();
            _mockCurrentUser = new Mock<ICurrentUserService>();
            _mockLogger = new Mock<ILogger<OrthoEngineCorrector>>();
            _client = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(15),
                BaseAddress = new Uri("http://localhost:11434")
            };
        }

        /// <summary>
        /// Function: ProcessTextAsync (Correction)
        /// Scenario: When correcting text with various models.
        /// Expected Result: Should return a successful correction without errors.
        /// </summary>
        [Theory]
        [InlineData("Ollama:llama3.2", ShortTextInput)]
        [InlineData("Ollama:llama3.1", ShortTextInput)]
        [InlineData("Ollama:llama3", ShortTextInput)]
        [InlineData("Ollama:mistral", ShortTextInput)]
        [InlineData("Ollama:gemma3", ShortTextInput)]
        [InlineData("Ollama:gemma3", LongTextInput)]
        [InlineData("Online:gemini-2.0-flash", ShortTextInput)]
        [InlineData("Online:gemini-2.0-flash", LongTextInput)]
       // [InlineData("Online:gemini-2.5-flash", ShortTextInput)]
        //[InlineData("Online:gemini-2.5-flash", LongTextInput)]
        public async Task ProcessTextAsync_ForCorrection_ShouldReturnSuccessfulResult(string modelName, string inputText)
        {
            // ARRANGE
            var engine = new OrthoEngineCorrector(_client, _mockSessionRepo.Object, _mockCurrentUser.Object, _mockLogger.Object)
            {
                ModelName = modelName
            };

            // ACT
            var result = await engine.ProcessTextAsync(inputText);

            // ASSERT
            _output.WriteLine($"--- Test for model: {modelName} ---");
            _output.WriteLine($"Input ({inputText.Length} chars): {inputText.Substring(0, Math.Min(inputText.Length, 50))}...");
            _output.WriteLine($"Output ({result.Length} chars): {result}");

            result.Should().NotContain("Erreur lors du traitement", "The processing should not result in an error.");
            engine.ModelName.Should().Be(modelName);
        }

        /// <summary>
        /// Function: ProcessTextAsync (Question to AI)
        /// Scenario: When asking a direct question to a model.
        /// Expected Result: Should return a successful answer without errors.
        /// </summary>
        [Theory]
        [InlineData("Online:gemini-2.0-flash", "QFIA Qui es tu?")]
        [InlineData("Online:gemini-2.0-flash", "QFIA Peut tu corriger...")] // Shortened for brevity
        public async Task ProcessTextAsync_ForQuestion_ShouldReturnSuccessfulResult(string modelName, string question)
        {
            // ARRANGE
            var engine = new OrthoEngineCorrector(_client, _mockSessionRepo.Object, _mockCurrentUser.Object, _mockLogger.Object)
            {
                ModelName = modelName
            };

            // ACT
            var result = await engine.ProcessTextAsync(question);

            // ASSERT
            _output.WriteLine($"--- Question for model: {modelName} ---");
            _output.WriteLine($"Input: {question}");
            _output.WriteLine($"Output: {result}");

            result.Should().NotContain("Erreur lors du traitement");
            engine.ModelName.Should().Be(modelName);
        }
    }
}

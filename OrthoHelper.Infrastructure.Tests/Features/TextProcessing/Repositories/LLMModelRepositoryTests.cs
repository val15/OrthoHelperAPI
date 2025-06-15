using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OrthoHelper.Domain.Features.TextCorrection.Ports.Repositories;

namespace OrthoHelper.Infrastructure.Features.TextProcessing.Repositories
{
    /// <summary>
    /// Contains INTEGRATION tests for the LLMModelRepository.
    /// IMPORTANT: These tests require a running Ollama instance at http://localhost:11434.
    /// They verify the actual interaction between the repository and the Ollama API.
    /// </summary>
    public class LLMModelRepositoryTests
    {
        // Real dependencies for the integration test
        private readonly HttpClient _client;
        private readonly ILLMModelRepository _llmModelRepository;

        // Mocked dependency, as logging behavior can be verified without a real logger
        private readonly Mock<ILogger<LLMModelRepository>> _mockLogger;

        public LLMModelRepositoryTests()
        {
            // ARRANGE (Shared setup)
            // 1. Create a real HttpClient configured to connect to the local Ollama service.
            _client = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(15),
                BaseAddress = new Uri("http://localhost:11434")
            };
            _mockLogger = new Mock<ILogger<LLMModelRepository>>();

            // 2. Create a real instance of the repository to be tested.
            _llmModelRepository = new LLMModelRepository(_client, _mockLogger.Object);
        }

        /// <summary>
        /// Function: GetAvailableLLMModelsAsync
        /// Scenario: When a connection to a running Ollama instance is successful and it has models installed.
        /// Expected Result: Should return a non-empty list of LLMModel objects.
        /// </summary>
        [Fact]
        public async Task GetAvailableLLMModelsAsync_WhenOllamaIsRunning_ShouldReturnNonEmptyList()
        {
            // ARRANGE
            // The setup is handled in the constructor. No additional arrangement is needed for this test.

            // ACT
            // Call the method that interacts with the live Ollama API.
            var result = await _llmModelRepository.GetAvailableLLMModelsAsync();

            // ASSERT
            // Verify that the result is not null and contains at least one model.
            // This confirms that the connection was successful and the API response was parsed correctly.
            result.Should().NotBeNull();
            result.Should().NotBeEmpty(); // A more expressive assertion than NotHaveCount(0)
        }
    }
}

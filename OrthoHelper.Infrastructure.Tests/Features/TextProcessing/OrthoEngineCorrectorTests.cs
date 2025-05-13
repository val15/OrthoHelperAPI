using Microsoft.Extensions.Logging;
using Moq;
using OrthoHelper.Domain.Features.Common.Ports;
using OrthoHelper.Domain.Features.TextCorrection.Ports.Repositories;
using OrthoHelper.Infrastructure.Features.TextProcessing;

namespace OrthoHelper.Infrastructure.Tests.Features.TextProcessing
{
    public class OrthoEngineCorrectorTests
    {
        private readonly HttpClient _client;
        private readonly Mock<ISessionRepository> _mockRepo;
        private readonly Mock<ICurrentUserService> _mockCurrentUserService;

        private readonly Mock<ILogger<OrthoEngineCorrector>> _mockLogger;

        public OrthoEngineCorrectorTests()
        {
            _client =   new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(15),
                BaseAddress = new Uri("http://localhost:11434")
            };

            _mockRepo = new Mock<ISessionRepository>();
            _mockCurrentUserService = new Mock<ICurrentUserService>();
            _mockLogger = new Mock<ILogger<OrthoEngineCorrector>>();
        }


        [Theory]
        [InlineData("Je veut un café", "Je veux un café.")]
        [InlineData("sa va", "Ça va.")]
        public void ProcessTextAsync_WithCommonErrors_ReturnsOutputText(string input, string expected)
        {
            // Arrange
            var engine = new OrthoEngineCorrector(_client, _mockRepo.Object, _mockCurrentUserService.Object, _mockLogger.Object);
            engine.ModelName = "Ollama:Gemma3";
            // Act
            var result = engine.ProcessTextAsync(input).Result; // .Result car la méthode est async

            // Assert
            Assert.True(result.Contains(expected));
        }
    }
}

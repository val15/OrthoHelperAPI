using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OrthoHelper.Domain.Features.TextCorrection.Ports.Repositories;

namespace OrthoHelper.Infrastructure.Features.TextProcessing.Repositories
{
    public class LLMModelRepositoryTests
    {
        private readonly ILLMModelRepository _llmModelRepository;
        private readonly HttpClient _client;
        private readonly Mock<ILogger<LLMModelRepository>> _mockLogger;
        public LLMModelRepositoryTests()
        {


            _client = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(15),
                BaseAddress = new Uri("http://localhost:11434")
            };
            _mockLogger = new Mock<ILogger<LLMModelRepository>>();


            _llmModelRepository = new LLMModelRepository(_client, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAvailableLLMModels_Should_Return_Correct_LLMModelList()
        {
            // Arrange
            //var expectedUser = new User { Username = "testuser", PasswordHash = "hash" };
            //_context.Users.Add(expectedUser);
            //await _context.SaveChangesAsync();

            // Act
            var result = await _llmModelRepository.GetAvailableLLMModelsAsync();

            // Assert
            //result.Should().BeEquivalentTo(expectedUser);
            result.Should().NotHaveCount(0);
        }
    }
}

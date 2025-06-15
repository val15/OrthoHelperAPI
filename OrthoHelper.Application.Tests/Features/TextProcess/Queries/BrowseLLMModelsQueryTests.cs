using AutoMapper;
using Moq;
using FluentAssertions;
using OrthoHelper.Application.Features.TextCorrection.Queries;
using OrthoHelper.Domain.Features.TextCorrection.Entities;
using OrthoHelper.Domain.Features.TextCorrection.Ports.Repositories;
using OrthoHelper.Application.Features.TextCorrection.Mappings;

namespace OrthoHelper.Application.Tests.Features.TextCorrection.Queries
{
    /// <summary>
    /// Contains unit tests for the BrowseLLMModelsQueryHandler.
    /// </summary>
    public class BrowseLLMModelsQueryHandlerTests
    {
        /// <summary>
        /// Tests that the Handle method correctly retrieves all available LLM models,
        /// maps them to DTOs, and returns them.
        /// </summary>
        [Fact]
        public async Task Handle_WhenModelsAreAvailable_ShouldReturnAllMappedModels()
        {
            // ARRANGE

            // 1. Mock the repository for Large Language Model (LLM) models.
            var mockRepo = new Mock<ILLMModelRepository>();

            // 2. Create a predefined list of LLMModel entities to be returned by the mock.
            var llmModels = new List<LLMModel>
        {
            new LLMModel("Ollama:Gemma"),
            new LLMModel("Online:gemini-2.5-flash"),
        };

            // 3. Configure the mock repository to return the predefined list
            //    when GetAvailableLLMModelsAsync is called.
            mockRepo.Setup(r => r.GetAvailableLLMModelsAsync())
                    .ReturnsAsync(llmModels);

            // 4. Set up an AutoMapper instance with the necessary profile for mapping
            //    LLMModel entities to their corresponding DTOs.
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<LLMModelProfile>(); // Assumes LLMModelProfile exists.
            });
            var mapper = mapperConfig.CreateMapper();

            // 5. Instantiate the handler to be tested, injecting the mock repository and the mapper.
            var handler = new BrowseLLMModelsQueryHandler(mockRepo.Object, mapper);

            // 6. Create an instance of the query to be processed by the handler.
            var query = new BrowseLLMModelsQuery();

            // ACT
            // Execute the handler with the query.
            var result = await handler.Handle(query, CancellationToken.None);

            // ASSERT
            // 1. Verify that the result contains the expected number of models.
            result.Should().HaveCount(2);

            // 2. Verify that the first model in the result was mapped correctly.
            result.First().ModelName.Should().Be("Ollama:Gemma");
        }
    }
}

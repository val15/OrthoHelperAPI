using AutoMapper;
using Moq;
using FluentAssertions;
using OrthoHelper.Application.Features.TextCorrection.Queries;
using OrthoHelper.Domain.Features.TextCorrection.Entities;
using OrthoHelper.Domain.Features.TextCorrection.Mappings;
using OrthoHelper.Domain.Features.TextCorrection.Ports.Repositories;
using OrthoHelper.Application.Features.TextCorrection.Mappings;

namespace OrthoHelper.Application.Tests.Features.TextCorrection.Queries
{
    public class BrowseLLMModelsQueryTest
    {

        [Fact]
        public async Task Handle_WhenCalled_LLMModels()
        {
            // Arrange
            var mockRepo = new Mock<ILLMModelRepository>();
            var LLMModels = new List<LLMModel>
       {
           new LLMModel("Ollama:Gemma"),
           new LLMModel("Online:gemini-2.0-flash")

       };

            mockRepo.Setup(r => r.GetAvailableLLMModelsAsync())
                    .ReturnsAsync(LLMModels);

            var handler = new BrowseLLMModelsQueryHandler(mockRepo.Object, new MapperConfiguration(cfg =>
                cfg.AddProfile<LLMModelProfile>()).CreateMapper());

            var query = new BrowseLLMModelsQuery();

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(2);
            result.First().ModelName.Should().Be("Ollama:Gemma");
        }
    }

}

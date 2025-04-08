using OrthoHelper.Domain.Features.TextCorrection.Entities;
using FluentAssertions;

namespace OrthoHelper.Domain.Tests.Features.TextCorrection.Entities
{
    public class LLMModelTests
    {

        [Fact]
        public void Release_WhenLotIsReserved_ShouldSetStatusToAvailable()
        {
            // Arrange
            var llmModel = new LLMModel("Ollama:Gemma");


            // Act


            // Assert
            llmModel.ModelName.Should().Be("Ollama:Gemma");
        }
    }
}

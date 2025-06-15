using OrthoHelper.Domain.Features.TextCorrection.Entities;
using FluentAssertions;

namespace OrthoHelper.Domain.Tests.Features.TextCorrection.Entities
{
    /// <summary>
    /// Contains unit tests for the LLMModel domain entity.
    /// </summary>
    public class LLMModelTests
    {
        /// <summary>
        /// Function: LLMModel Constructor
        /// Scenario: When creating an instance with a valid model name string.
        /// Expected Result: The ModelName property should be assigned correctly.
        /// </summary>
        [Fact]
        public void Constructor_WithValidModelName_ShouldAssignModelNameProperty()
        {
            // ARRANGE
            // Define the input string for the model name.
            var modelNameString = "Ollama:Gemma";

            // ACT
            // Create a new instance of the LLMModel.
            var llmModel = new LLMModel(modelNameString);

            // ASSERT
            // Verify that the ModelName property was set to the value provided in the constructor.
            llmModel.ModelName.Should().Be(modelNameString);
        }
    }
}

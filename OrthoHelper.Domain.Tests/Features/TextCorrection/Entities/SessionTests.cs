// OrthoHelper.Domain.Tests/Entities/CorrectionSessionTests.cs
using FluentAssertions;
using OrthoHelper.Domain.Features.TextCorrection.Entities;
using OrthoHelper.Domain.Features.TextCorrection.Exceptions;

namespace OrthoHelper.Domain.Tests.Features.TextCorrection.Entities
{

    /// <summary>
    /// Contains unit tests for the Session domain entity, focusing on its creation,
    /// validation, and state-transition logic.
    /// </summary>
    public class SessionTests // Renamed to reflect the class being tested
    {
        /// <summary>
        /// Function: Session.Create (Factory)
        /// Scenario: When creating a session with valid, non-empty text.
        /// Expected Result: Should initialize properties with correct default values.
        /// </summary>
        [Fact]
        public void Create_WithValidText_ShouldInitializePropertiesCorrectly()
        {
            // ARRANGE
            // Define the valid input text for the session.
            var inputText = "Hello, I wants a coffee."; // Intentionally incorrect grammar

            // ACT
            // Call the factory method to create the session instance.
            var session = Session.Create(inputText);

            // ASSERT
            // Verify that the initial state is set as expected.
            session.InputText.Should().Be(inputText);
            session.Status.Should().Be(CorrectionStatus.Pending);
            session.Id.Should().NotBe(Guid.Empty);
            session.OutputText.Should().BeNull(); // An explicit check for initial state
        }

        /// <summary>
        /// Function: Session.Create (Factory)
        /// Scenario: When attempting to create a session with empty text.
        /// Expected Result: Should throw an InvalidTextException.
        /// </summary>
        [Fact]
        public void Create_WithEmptyText_ShouldThrowInvalidTextException()
        {
            // ARRANGE
            // Define the action that attempts to create a session with invalid input.
            Action act = () => Session.Create("");

            // ACT & ASSERT
            // Verify that the action throws the expected exception.
            act.Should().Throw<InvalidTextException>();
        }

        /// <summary>
        /// Function: ApplyCorrection (Instance Method)
        /// Scenario: When applying a correction to a session that is in 'Pending' status.
        /// Expected Result: Should update the OutputText and change the status to 'Completed'.
        /// </summary>
        [Fact]
        public void ApplyCorrection_WhenStatusIsPending_ShouldUpdateTextAndStatus()
        {
            // ARRANGE
            // Create a session in the 'Pending' state.
            var session = Session.Create("Some test text");
            var correctedText = "Some tested text";

            // ACT
            // Apply the correction.
            session.ApplyCorrection(correctedText);

            // ASSERT
            // Verify that the session's state and properties have been updated correctly.
            session.OutputText.Should().Be(correctedText);
            session.Status.Should().Be(CorrectionStatus.Completed);
        }

        /// <summary>
        /// Function: ApplyCorrection (Instance Method)
        /// Scenario: When attempting to apply a correction to a session that is already 'Completed'.
        /// Expected Result: Should throw an InvalidOperationException.
        /// </summary>
        [Fact]
        public void ApplyCorrection_WhenStatusIsCompleted_ShouldThrowInvalidOperationException()
        {
            // ARRANGE
            // Create a session and immediately complete it.
            var session = Session.Create("Initial text");
            session.ApplyCorrection("First correction"); // This moves status to Completed

            // Define the action that attempts to apply a second correction.
            Action act = () => session.ApplyCorrection("Second correction");

            // ACT & ASSERT
            // Verify that attempting to modify a completed session throws an exception.
            act.Should().Throw<InvalidOperationException>();
        }
    }
}
// OrthoHelper.Domain.Tests/Entities/CorrectionSessionTests.cs
using OrthoHelper.Domain.Features.TextCorrection.Entities;
using OrthoHelper.Domain.Features.TextCorrection.Exceptions;

namespace OrthoHelper.Domain.Tests.Features.TextCorrection.Entities
{


    public class CorrectionSessionTests
    {
        [Fact]
        public void Create_WithValidText_InitializesCorrectly()
        {
            // Arrange
            var text = "Bonjour, je veut un café";

            // Act
            var session = Session.Create(text);

            // Assert
            Assert.Equal(text, session.InputText);
            Assert.Equal(CorrectionStatus.Pending, session.Status);
            Assert.NotEqual(Guid.Empty, session.Id);
        }

        [Fact]
        public void Create_WithEmptyText_ThrowsInvalidTextException()
        {
            // Arrange
            var emptyText = "";

            // Act & Assert
            Assert.Throws<InvalidTextException>(() => Session.Create(emptyText));
        }

        [Fact]
        public void ApplyCorrection_WhenPending_UpdatesOutputText()
        {
            // Arrange
            var session = Session.Create("Test");
            var correctedText = "Tested";

            // Act
            session.ApplyCorrection(correctedText);

            // Assert
            Assert.Equal(correctedText, session.OutputText);
            Assert.Equal(CorrectionStatus.Completed, session.Status);
        }

        [Fact]
        public void ApplyCorrection_WhenAlreadyCompleted_ThrowsException()
        {
            // Arrange
            var session = Session.Create("Test");
            session.ApplyCorrection("Tested");

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => session.ApplyCorrection("New correction"));
        }
    }
}
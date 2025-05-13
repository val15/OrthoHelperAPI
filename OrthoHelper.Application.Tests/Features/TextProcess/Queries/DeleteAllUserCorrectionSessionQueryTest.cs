using FluentAssertions;
using Moq;
using OrthoHelper.Application.Features.TextCorrection.Queries;
using OrthoHelper.Domain.Features.TextCorrection.Entities;
using OrthoHelper.Domain.Features.TextCorrection.Ports.Repositories;

namespace OrthoHelper.Application.Tests.Features.TextCorrection.Queries
{
    public class DeleteAllUserCorrectionSessionQueryTest
    {
        [Fact]
        public async Task Handle_WhenCalled_CorrectionSessions()
        {
            // Arrange
            var mockRepo = new Mock<ISessionRepository>();
            var correctionSessions = new List<Session>
        {
            Session.Create("textIncorrect1"),
            Session.Create("textIncorrect2")
        };

            mockRepo.Setup(r => r.DeleteAllUserCorrectionSessionsAsync())
                .ReturnsAsync(correctionSessions.Count);

            var handler = new DeleteAllUserCorrectionSessionQueryHandler(mockRepo.Object);
            var query = new DeleteAllUserSessionQuery();

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Equals(2);
        }
    }
}

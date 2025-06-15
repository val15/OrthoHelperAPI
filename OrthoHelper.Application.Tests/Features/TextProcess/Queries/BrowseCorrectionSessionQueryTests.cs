using AutoMapper;
using FluentAssertions;
using Moq;
using OrthoHelper.Application.Features.TextCorrection.Queries;
using OrthoHelper.Domain.Features.TextCorrection.Entities;
using OrthoHelper.Domain.Features.TextCorrection.Mappings;
using OrthoHelper.Domain.Features.TextCorrection.Ports.Repositories;

namespace OrthoHelper.Application.Tests.Features.TextCorrection.Queries
{
    /// <summary>
    /// Contains unit tests for the BrowseCorrectionSessionQueryHandler.
    /// </summary>
    public class BrowseCorrectionSessionQueryHandlerTests
    {
        /// <summary>
        /// Tests that the Handle method correctly retrieves all existing sessions from the repository,
        /// maps them to DTOs, and returns the collection.
        /// </summary>
        [Fact]
        public async Task Handle_WhenSessionsExist_ShouldReturnAllMappedSessions()
        {
            // ARRANGE

            // 1. Mock the session repository dependency to control its behavior during the test.
            var mockRepo = new Mock<ISessionRepository>();

            // 2. Create a list of domain entities (Session) to be returned by the mock repository.
            var correctionSessions = new List<Session>
        {
            Session.Create("textIncorrect1"),
            Session.Create("textIncorrect2")
        };

            // 3. Configure the mock repository to return our predefined list of sessions
            //    when GetSessionsAsync is called.
            mockRepo.Setup(r => r.GetSessionsAsync())
                    .ReturnsAsync(correctionSessions);

            // 4. Set up an AutoMapper instance with the required profile to map Session entities to Session DTOs.
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<SessionProfile>(); // Assumes a SessionProfile defines the mapping logic.
            });
            var mapper = mapperConfig.CreateMapper();

            // 5. Instantiate the handler to be tested, injecting the mocked repository and the configured mapper.
            var handler = new BrowseCorrectionSessionQueryHandler(mockRepo.Object, mapper);

            // 6. Create an instance of the query. For this handler, it doesn't carry any parameters.
            var query = new BrowseSessionQuery();

            // ACT

            // Execute the handler with the query and a cancellation token.
            var result = await handler.Handle(query, CancellationToken.None);

            // ASSERT

            // 1. Verify that the result contains the expected number of items (2 in this case).
            result.Should().HaveCount(2);

            // 2. Verify that the content of the first item in the result was correctly mapped from the source entity.
            result.First().InputText.Should().Be("textIncorrect1");
        }
    }
}

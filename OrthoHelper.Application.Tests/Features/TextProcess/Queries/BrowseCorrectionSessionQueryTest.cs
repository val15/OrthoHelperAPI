﻿using AutoMapper;
using FluentAssertions;
using Moq;
using OrthoHelper.Application.Features.TextCorrection.Queries;
using OrthoHelper.Domain.Features.TextCorrection.Entities;
using OrthoHelper.Domain.Features.TextCorrection.Mappings;
using OrthoHelper.Domain.Features.TextCorrection.Ports.Repositories;

namespace OrthoHelper.Application.Tests.Features.TextCorrection.Queries
{
    public class BrowseCorrectionSessionQueryTest
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

            mockRepo.Setup(r => r.GetCorrectionSessionsAsync())
                    .ReturnsAsync(correctionSessions);

            var handler = new BrowseCorrectionSessionQueryHandler(mockRepo.Object, new MapperConfiguration(cfg =>
                cfg.AddProfile<SessionProfile>()).CreateMapper());

            var query = new BrowseSessionQuery();

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(2);
            result.First().InputText.Should().Be("textIncorrect1");
        }
    }
}

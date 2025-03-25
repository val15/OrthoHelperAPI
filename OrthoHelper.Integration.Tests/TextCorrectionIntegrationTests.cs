using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using OrthoHelper.Application.Features.TextCorrection.DTOs;
using Xunit;

namespace OrthoHelper.Integration.Tests;

public class TextCorrectionIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public TextCorrectionIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            // Force la copie des fichiers nécessaires
             builder.UseSolutionRelativeContentRoot("C:/inProgress/sdz/update2025/IA/OrthoHelper/OrthoHelperAPI/OrthoHelperAPI");
           // builder.UseSolutionRelativeContentRoot("..\\OrthoHelperAPI");

        });
    }

    [Fact]
    public async Task POST_CorrectText_ReturnsCorrectedText()
    {
        // Arrange
        var client = _factory.CreateClient();
        var input = new CorrectTextInputDto("Je veut un café");
        var content = new StringContent(
            JsonSerializer.Serialize(input),
            Encoding.UTF8,
            "application/json"
        );

        // Act
        var response = await client.PostAsync("/api/text-correction", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<CorrectTextOutputDto>(
            responseContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        result!.CorrectedText.Should().Be("Je veux un café");
    }

    [Fact]
    public async Task POST_EmptyText_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var input = new CorrectTextInputDto("");
        var content = new StringContent(
            JsonSerializer.Serialize(input),
            Encoding.UTF8,
            "application/json"
        );

        // Act
        var response = await client.PostAsync("/api/text-correction", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
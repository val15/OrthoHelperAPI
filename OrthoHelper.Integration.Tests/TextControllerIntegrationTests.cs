using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using OrthoHelper.Application.Features.TextCorrection.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using OrthoHelper.Application.Features.TextTranslation.DTOs;
using static OrthoHelper.Domain.Features.TextCorrection.Entities.Session;

namespace OrthoHelper.Integration.Tests
{
    public class TextControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public TextControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();

            // Authentification (remplacez par votre méthode d'obtention de token)
            var token = GetJwtTokenAsync("testUser", "testUserPass").Result;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        private async Task<string> GetJwtTokenAsync(string username, string password)
        {
            var loginDto = new { Username = username, Password = password };
            var response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                // Register si l'utilisateur n'existe pas
                var registerDto = new { Username = username, Password = password };
                await _client.PostAsJsonAsync("/api/auth/register", registerDto);
                response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);
            }
            var loginResult = await response.Content.ReadFromJsonAsync<LoginUserResponse>();
            return loginResult!.Token;
        }
        [Fact]
        public async Task BrowseAvailableModels_ShouldReturnOkWithModels()
        {
            var response = await _client.GetAsync("/api/text/Models");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var models = await response.Content.ReadFromJsonAsync<List<LLMModelOutputDto>>();
            models.Should().NotBeNull();
            models.Should().AllSatisfy(m => m.ModelName.Should().NotBeNullOrEmpty());
        }

        [Fact]
        public async Task CorrectText_ShouldReturnOkWithCorrection()
        {
            var input = new InputTextDto("Je veut un café", "Online:gemini-2.0-flash");
            var response = await _client.PostAsJsonAsync("/api/text/correct", input);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var output = await response.Content.ReadFromJsonAsync<OutputTextDto>();
            output.Should().NotBeNull();
            output.InputText.Should().Be("Je veut un café");
            output.OutputText.Should().NotBeNullOrEmpty();
            output.Type.Should().Be(MessageType.Corrector);
        }

        [Fact]
        public async Task TranslateText_ShouldReturnOkWithTranslation()
        {
            var input = new InputTextDto("Hello", "Online:gemini-2.0-flash");
            var response = await _client.PostAsJsonAsync("/api/text/translate", input);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var output = await response.Content.ReadFromJsonAsync<OutputTextDto>();
            output.Should().NotBeNull();
            output.InputText.Should().Be("Hello");
            output.OutputText.Should().NotBeNullOrEmpty();
            output.Type.Should().Be(MessageType.Translator);
        }

        [Fact]
        public async Task TranslateHtmlFile_sample_ShouldReturnOkWithPaths()
        {
            var input = new HtmlTranslationInputDto
            {
                HtmlFilePath = "TestFiles/sample.html",
                ModelName = "Online:gemini-2.0-flash"
            };
            var response = await _client.PostAsJsonAsync("/api/text/translate-html-file", input);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var output = await response.Content.ReadFromJsonAsync<HtmlTranslationOutputDto>();
            output.Should().NotBeNull();
            output.OriginalHtmlPath.Should().Be(input.HtmlFilePath);
            output.TranslatedHtmlPath.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task TranslateHtmlFile_lotteryletters_converted_ShouldReturnOkWithPaths()
        {
            var input = new HtmlTranslationInputDto
            {
                HtmlFilePath = "TestFiles/lotteryletters-converted.html",
                ModelName = "Online:gemini-2.0-flash"
            };
            var response = await _client.PostAsJsonAsync("/api/text/translate-html-file", input);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var output = await response.Content.ReadFromJsonAsync<HtmlTranslationOutputDto>();
            output.Should().NotBeNull();
            output.OriginalHtmlPath.Should().Be(input.HtmlFilePath);
            output.TranslatedHtmlPath.Should().NotBeNullOrEmpty();
        }


        [Fact]
        public async Task BrowseMessages_ShouldReturnListOfMessages()
        {
            // Act
            var response = await _client.GetAsync("/api/text/Messages");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var messages = await response.Content.ReadFromJsonAsync<List<OutputTextDto>>();
            messages.Should().NotBeNull();
            messages.Should().BeOfType<List<OutputTextDto>>();
            // Optionally, check the content of messages
            // messages.Should().Contain(m => !string.IsNullOrEmpty(m.InputText));
        }
    }

    // DTO pour la désérialisation du token
    public class LoginUserResponse
    {
        public string Token { get; set; }
    }
}

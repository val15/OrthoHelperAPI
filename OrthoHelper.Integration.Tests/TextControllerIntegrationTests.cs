using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using OrthoHelper.Application.Features.TextCorrection.DTOs;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;
using OrthoHelper.Application.Features.TextTranslation.DTOs;
using static OrthoHelper.Domain.Features.TextCorrection.Entities.Session;

namespace OrthoHelper.Integration.Tests
{
    /// <summary>
    /// Contains integration tests for the TextController, verifying the behavior of all its endpoints.
    /// This class uses a WebApplicationFactory to bootstrap the application in-memory.
    /// </summary>
    public class TextControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
    {
        private readonly HttpClient _client;

        public TextControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        /// <summary>
        /// Performs asynchronous initialization before any tests in the class are run.
        /// This is used here to log in and set the authorization token for all subsequent requests.
        /// </summary>
        public async Task InitializeAsync()
        {
            var token = await GetJwtTokenAsync("testUser", "testUserPass");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        /// <summary>
        /// Performs asynchronous cleanup after all tests in the class have run.
        /// </summary>
        public Task DisposeAsync() => Task.CompletedTask;

        /// <summary>
        /// Endpoint: GET /api/text/Models
        /// Scenario: When an authenticated user requests the list of available models.
        /// Expected Result: Returns HTTP 200 OK with a non-empty list of models.
        /// </summary>
        [Fact]
        public async Task GetModels_WhenCalled_ReturnsOkWithListOfModels()
        {
            // Act
            var response = await _client.GetAsync("/api/text/Models");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var models = await response.Content.ReadFromJsonAsync<List<LLMModelOutputDto>>();
            models.Should().NotBeNullOrEmpty();
            models.Should().AllSatisfy(m => m.ModelName.Should().NotBeNullOrEmpty());
        }

        /// <summary>
        /// Endpoint: POST /api/text/correct
        /// Scenario: When submitting valid text for correction.
        /// Expected Result: Returns HTTP 200 OK with the corrected text.
        /// </summary>
        [Fact]
        public async Task PostCorrect_WithValidInput_ReturnsOkWithCorrection()
        {
            // Arrange
            var input = new InputTextDto("je veu n cafe", "Online:gemini-2.0-flash"); // "I wants a coffee"

            // Act
            var response = await _client.PostAsJsonAsync("/api/text/correct", input);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var output = await response.Content.ReadFromJsonAsync<OutputTextDto>();
            output.Should().NotBeNull();
            output.InputText.Should().Be(input.Text);
            output.OutputText.Should().NotBeNullOrEmpty();
            output.Type.Should().Be(MessageType.Corrector);
        }

        /// <summary>
        /// Endpoint: POST /api/text/translate
        /// Scenario: When submitting valid text for translation.
        /// Expected Result: Returns HTTP 200 OK with the translated text.
        /// </summary>
        [Fact]
        public async Task PostTranslate_WithValidInput_ReturnsOkWithTranslation()
        {
            // Arrange
            var input = new InputTextDto("Hello you", "Online:gemini-2.0-flash");

            // Act
            var response = await _client.PostAsJsonAsync("/api/text/translate", input);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var output = await response.Content.ReadFromJsonAsync<OutputTextDto>();
            output.Should().NotBeNull();
            output.InputText.Should().Be(input.Text);
            output.OutputText.Should().NotBeNullOrEmpty();
            output.Type.Should().Be(MessageType.Translator);
        }

        /// <summary>
        /// Endpoint: POST /api/text/translate-html-file
        /// Scenario: When submitting a valid HTML file for translation.
        /// Expected Result: Returns HTTP 200 OK with paths to the original and translated files.
        /// </summary>
        [Theory]
        [InlineData("TestFiles/sample.html")]
      //  [InlineData("TestFiles/lotteryletters-converted.html")]
        public async Task PostTranslateHtmlFile_WithValidFile_ReturnsOkWithPaths(string filePath)
        {
            // Arrange
            var input = new HtmlTranslationInputDto
            {
                HtmlFilePath = filePath,
                ModelName = "Online:gemini-2.0-flash"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/text/translate-html-file", input);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var output = await response.Content.ReadFromJsonAsync<HtmlTranslationOutputDto>();
            output.Should().NotBeNull();
            output.OriginalHtmlPath.Should().Be(input.HtmlFilePath);
            output.TranslatedHtmlPath.Should().NotBeNullOrEmpty();
        }

        /// <summary>
        /// Endpoint: GET /api/text/Messages
        /// Scenario: When an authenticated user requests their message history.
        /// Expected Result: Returns HTTP 200 OK with a list of messages.
        /// </summary>
        [Fact]
        public async Task GetMessages_WhenCalled_ReturnsOkWithListOfMessages()
        {
            // Act
            var response = await _client.GetAsync("/api/text/Messages");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var messages = await response.Content.ReadFromJsonAsync<List<OutputTextDto>>();
            messages.Should().NotBeNull();
            messages.Should().BeOfType<List<OutputTextDto>>();
        }

        /// <summary>
        /// Helper method to get a JWT token. It will register the user if they don't exist,
        /// then log them in. This makes the test suite self-contained and runnable.
        /// </summary>
        private async Task<string> GetJwtTokenAsync(string username, string password)
        {
            var loginDto = new { Username = username, Password = password };
            var response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                // Register the user if they don't exist, then try logging in again.
                var registerDto = new { Username = username, Password = password };
                await _client.PostAsJsonAsync("/api/auth/register", registerDto);
                response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);
            }

            response.EnsureSuccessStatusCode();
            var loginResult = await response.Content.ReadFromJsonAsync<LoginUserResponse>();
            return loginResult!.Token;
        }
    }

    /// <summary>
    /// DTO for deserializing the login response to extract the token.
    /// </summary>
    public class LoginUserResponse
    {
        public string Token { get; set; }
    }
}

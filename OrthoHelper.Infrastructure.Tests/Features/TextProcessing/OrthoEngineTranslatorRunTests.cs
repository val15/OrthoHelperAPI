using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Moq;
using OrthoHelper.Domain.Features.Common.Ports;
using OrthoHelper.Domain.Features.TextCorrection.Ports.Repositories;
using OrthoHelper.Infrastructure.Features.TextProcessing;
using Xunit.Abstractions;

namespace OrthoHelper.Infrastructure.Tests.Features.TextProcessing
{


    public class OrthoEngineTranslatorRunTests
    {


        private readonly HttpClient _client;
        private readonly Mock<ISessionRepository> _mockCorrectionSessionRepository;
        private readonly Mock<ICurrentUserService> _mockCurrentUserService;

        private readonly Mock<ILogger<OrthoEngineTranslator>> _mockLogger;


        private readonly ITestOutputHelper _output;
        private readonly string inputText = "Hello";
        public OrthoEngineTranslatorRunTests(ITestOutputHelper output)
        {
            _client = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(15),
                BaseAddress = new Uri("http://localhost:11434")
            };

            _mockCorrectionSessionRepository = new Mock<ISessionRepository>();
            _mockCurrentUserService = new Mock<ICurrentUserService>();
            _mockLogger = new Mock<ILogger<OrthoEngineTranslator>>();




            _output = output;
        }
        [Fact]
        public async Task ProcessTextAsync_ReturnsTranslated_gemini20_Text()
        {
            // Arrange
            var input = inputText;

            var engine = new OrthoEngineTranslator(_client, _mockCorrectionSessionRepository.Object, _mockCurrentUserService.Object, _mockLogger.Object);
            engine.ModelName = "Online:gemini-2.0-flash";
            //_chatServiceMock.Setup(cs => cs.GetChatMessageContentAsync(It.IsAny<ChatHistory>()))
            //    .ReturnsAsync(new ChatMessage { Content = correctedText });


            // Act
            var result = await engine.ProcessTextAsync(input);

            // Assert
            //Assert.Equal(correctedText, result);
            _output.WriteLine($"Input : {input}");
            _output.WriteLine($"Output : {result}");
            Assert.Equal("Online:gemini-2.0-flash", engine.ModelName);
            Assert.DoesNotContain("Erreur lors du traitement", result);
        }

        [Fact]
        public async Task ProcessTextAsync_ReturnsTranslated_gemma3_Text()
        {
            // Arrange
            var input = inputText;

            var engine = new OrthoEngineTranslator(_client, _mockCorrectionSessionRepository.Object, _mockCurrentUserService.Object, _mockLogger.Object);
            engine.ModelName = "Ollama:gemma3";
            //_chatServiceMock.Setup(cs => cs.GetChatMessageContentAsync(It.IsAny<ChatHistory>()))
            //    .ReturnsAsync(new ChatMessage { Content = correctedText });


            // Act
            var result = await engine.ProcessTextAsync(input);

            // Assert
            //Assert.Equal(correctedText, result);
            _output.WriteLine($"Input : {input}");
            _output.WriteLine($"Output : {result}");
            Assert.Equal("Ollama:gemma3", engine.ModelName);
            Assert.DoesNotContain("Erreur lors du traitement", result);
        }


        [Fact]
        public async Task ProcessTextAsync_ReturnsTranslated_gemma3_long_Text()
        {
            // Arrange
            var input = "Friend’s note was among the first of the torrent of letters that arrived at The New Yorker in the wake of “The Lottery”—the most mail the magazine had ever received in response to a work of fiction. Jackson’s story, in which the residents of an unidentified American village participate in an annual rite of stoning to death a person chosen among them by drawing lots, would quickly become one of the best known and most frequently anthologized short stories in English. “The Lottery” has been adapted for stage, television, opera, and ballet; it was even featured in an episode of “The Simpsons.” By now it is so familiar that it is hard to remember how shocking it originally seemed: “outrageous,”\r\n\r\n“gruesome,” or just “utterly pointless,” in the words of some of the readers who were moved to write. When I spoke to Friend recently—she is the only one of the letter writers I could track down who is still alive—she still remembered how upsetting she had found “The Lottery.” “I don’t know how anyone approved of that story,” she told me";

            var engine = new OrthoEngineTranslator(_client, _mockCorrectionSessionRepository.Object, _mockCurrentUserService.Object, _mockLogger.Object);
            engine.ModelName = "Ollama:gemma3";
            //_chatServiceMock.Setup(cs => cs.GetChatMessageContentAsync(It.IsAny<ChatHistory>()))
            //    .ReturnsAsync(new ChatMessage { Content = correctedText });


            // Act
            var result = await engine.ProcessTextAsync(input);

            // Assert
            //Assert.Equal(correctedText, result);
            _output.WriteLine($"Input : {input}");
            _output.WriteLine($"Output : {result}");
            Assert.Equal("Ollama:gemma3", engine.ModelName);
            Assert.DoesNotContain("Erreur lors du traitement", result);
        }

    }
}

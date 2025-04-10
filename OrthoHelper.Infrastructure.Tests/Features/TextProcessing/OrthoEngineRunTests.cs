using Castle.Core.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using OrthoHelper.Domain.Features.Common.Ports;
using OrthoHelper.Domain.Features.TextCorrection.Ports.Repositories;
using OrthoHelper.Infrastructure.Features.TextProcessing;
using Tools;
using Xunit.Abstractions;

namespace OrthoHelper.Infrastructure.Tests.Features.TextProcessing
{
    public class OrthoEngineRunTests
    {
        private readonly HttpClient _client;
        private readonly Mock<ICorrectionSessionRepository> _mockRepo;
        private readonly Mock<ICurrentUserService> _mockCurrentUserService;

        private readonly Mock<ILogger<OrthoEngine>> _mockLogger;


        private readonly ITestOutputHelper _output;
        private readonly string inputText = "Salu Henitsoa, je viens vert toi a la demande de Hafalina pour donné mon coup de main sur l'application novyparking que tu develope";
        public OrthoEngineRunTests(ITestOutputHelper output)
        {
            _client = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(15),
                BaseAddress = new Uri("http://localhost:11434")
            };

            _mockRepo = new Mock<ICorrectionSessionRepository>();
            _mockCurrentUserService = new Mock<ICurrentUserService>();
            _mockLogger = new Mock<ILogger<OrthoEngine>>();




            _output = output;
        }

        [Fact]
        public void OrthoService_Initialization_Success()
        {
            // Arrange

            // Act

            var engine = new OrthoEngine(_client, _mockRepo.Object, _mockCurrentUserService.Object, _mockLogger.Object);
            engine.ModelName = "Ollama:llama3.2";
            // Assert
            Assert.NotNull(engine);
        }

        [Fact]
        public async Task ProcessTextAsync_ReturnsCorrected_llama32_Text()
        {
            // Arrange
            var input = inputText;

            var modelName = "Ollama:llama3.2";
            var engine = new OrthoEngine(_client, _mockRepo.Object, _mockCurrentUserService.Object, _mockLogger.Object);
            engine.ModelName = modelName;
            //_chatServiceMock.Setup(cs => cs.GetChatMessageContentAsync(It.IsAny<ChatHistory>()))
            //    .ReturnsAsync(new ChatMessage { Content = correctedText });

            // Act
            var result = await engine.ProcessTextAsync(input);

            // Assert
            //Assert.Equal(correctedText, result);
            Assert.Equal(modelName, engine.ModelName);
            _output.WriteLine($"Input : {input}");
            _output.WriteLine($"Output : {result}");



        }

        [Fact]
        public async Task ProcessTextAsync_ReturnsCorrected_llama31_Text()
        {
            // Arrange
            var input = inputText;

            var modelName = "Ollama:llama3.1";
            var engine = new OrthoEngine(_client, _mockRepo.Object, _mockCurrentUserService.Object, _mockLogger.Object);
            engine.ModelName = modelName;
            //_chatServiceMock.Setup(cs => cs.GetChatMessageContentAsync(It.IsAny<ChatHistory>()))
            //    .ReturnsAsync(new ChatMessage { Content = correctedText });

            // Act
            var result = await engine.ProcessTextAsync(input);

            // Assert
            //Assert.Equal(correctedText, result);
            Assert.Equal(modelName, engine.ModelName);
            _output.WriteLine($"Input : {input}");
            _output.WriteLine($"Output : {result}");



        }


        [Fact]
        public async Task ProcessTextAsync_ReturnsCorrected_llama3_Text()
        {
            // Arrange
            var input = inputText;

            var modelName = "Ollama:llama3";
            var engine = new OrthoEngine(_client, _mockRepo.Object, _mockCurrentUserService.Object, _mockLogger.Object);
            engine.ModelName = modelName;
            //_chatServiceMock.Setup(cs => cs.GetChatMessageContentAsync(It.IsAny<ChatHistory>()))
            //    .ReturnsAsync(new ChatMessage { Content = correctedText });

            // Act
            var result = await engine.ProcessTextAsync(input);

            // Assert
            //Assert.Equal(correctedText, result);
            _output.WriteLine($"Input : {input}");
            _output.WriteLine($"Output : {result}");
            Assert.Equal(modelName, engine.ModelName);



        }

        [Fact]
        public async Task ProcessTextAsync_ReturnsCorrected_mistral_Text()
        {
            // Arrange
            var input = inputText;

            var modelName = "Ollama:mistral";
            var engine = new OrthoEngine(_client, _mockRepo.Object, _mockCurrentUserService.Object, _mockLogger.Object);
            engine.ModelName = modelName;
            //_chatServiceMock.Setup(cs => cs.GetChatMessageContentAsync(It.IsAny<ChatHistory>()))
            //    .ReturnsAsync(new ChatMessage { Content = correctedText });

            // Act
            var result = await engine.ProcessTextAsync(input);

            // Assert
            //Assert.Equal(correctedText, result);
            Assert.Equal(modelName, engine.ModelName);
            _output.WriteLine($"Input : {input}");
            _output.WriteLine($"Output : {result}");



        }


        [Fact]
        public async Task ProcessTextAsync_ReturnsCorrected_gemma3_Text()
        {
            // Arrange
            var input = inputText;

            var modelName = "Ollama:gemma3";
            var engine = new OrthoEngine(_client, _mockRepo.Object, _mockCurrentUserService.Object, _mockLogger.Object);
            engine.ModelName = modelName;
            //_chatServiceMock.Setup(cs => cs.GetChatMessageContentAsync(It.IsAny<ChatHistory>()))
            //    .ReturnsAsync(new ChatMessage { Content = correctedText });

            // Act
            var result = await engine.ProcessTextAsync(input);

            // Assert
            //Assert.Equal(correctedText, result);
            _output.WriteLine($"Input : {input}");
            _output.WriteLine($"Output : {result}");

            Assert.Equal(modelName, engine.ModelName);


            Assert.DoesNotContain("Erreur lors du traitement", result);
        }

        [Fact]
        public async Task ProcessTextAsync_ReturnsCorrected_gemma312_Text()
        {
            // Arrange
            var input = inputText;

            var modelName = "Ollama:gemma3:12b";
            var engine = new OrthoEngine(_client, _mockRepo.Object, _mockCurrentUserService.Object, _mockLogger.Object);
            engine.ModelName = modelName;
            //_chatServiceMock.Setup(cs => cs.GetChatMessageContentAsync(It.IsAny<ChatHistory>()))
            //    .ReturnsAsync(new ChatMessage { Content = correctedText });

            // Act
            var result = await engine.ProcessTextAsync(input);

            // Assert
            //Assert.Equal(correctedText, result);
            _output.WriteLine($"Input : {input}");
            _output.WriteLine($"Output : {result}");


            Assert.Equal(modelName, engine.ModelName);

            Assert.DoesNotContain("Erreur lors du traitement", result);
        }

        [Fact]
        public async Task ProcessTextAsync_ReturnsCorrected_gemini20_Text()
        {
            // Arrange
            var input = inputText;

            var engine = new OrthoEngine(_client, _mockRepo.Object, _mockCurrentUserService.Object, _mockLogger.Object);
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
        public void TestCharacterDifference()
        {
            // Arrange
            string text1 = inputText;
            string text2 = "Salut Henitsoa, je viens vous aider à la demande de Hafalina pour donner mon coup de main sur l'application NovyParking que vous développez.";

            // Act
            string result = TextDiffHelper.GenerateCharacterDiff(text1, text2);

            // Assert
            _output.WriteLine("Différences HTML générées :");
            _output.WriteLine(result);

            Assert.Contains("text-decoration: line-through", result);
            Assert.Contains("color: blue", result);
        }
    }
}

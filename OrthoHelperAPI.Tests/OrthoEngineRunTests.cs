using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.ChatCompletion;
using Moq;
using OrthoHelperAPI.Services;
using Tools;
using Xunit.Abstractions;

namespace OrthoHelperAPI.Tests
{
    public class OrthoServiceTests
    {
        private readonly ITestOutputHelper _output;
        private readonly Mock<ILogger<OrthoService>> _loggerMock;
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IChatCompletionService> _chatServiceMock;
        private readonly string inputText = "Salu Henitsoa, je viens vert toi a la demande de Hafalina pour donné mon coup de main sur l'application novyparking que tu develope";
        public OrthoServiceTests(ITestOutputHelper output)
        {
            _loggerMock = new Mock<ILogger<OrthoService>>();
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _configurationMock = new Mock<IConfiguration>();
            _chatServiceMock = new Mock<IChatCompletionService>();
            _output = output;
        }

        [Fact]
        public void OrthoService_Initialization_Success()
        {
            // Arrange
            _configurationMock.Setup(c => c["ModelSettings:ModelName"]).Returns("llama3.2");
            _configurationMock.Setup(c => c["ModelSettings:Address"]).Returns("http://localhost:11434");

            // Act
            var service = new OrthoService(_loggerMock.Object, _httpClientFactoryMock.Object, _configurationMock.Object);

            // Assert
            Assert.NotNull(service);
        }

        [Fact]
        public async Task ProcessTextAsync_ReturnsCorrected_llama32_Text()
        {
            // Arrange
            var input = inputText;
            _configurationMock.Setup(c => c["ModelSettings:ModelName"]).Returns("Ollama:llama3.2");
            _configurationMock.Setup(c => c["ModelSettings:Address"]).Returns("http://localhost:11434");

            var service = new OrthoService(_loggerMock.Object, _httpClientFactoryMock.Object, _configurationMock.Object);

            //_chatServiceMock.Setup(cs => cs.GetChatMessageContentAsync(It.IsAny<ChatHistory>()))
            //    .ReturnsAsync(new ChatMessage { Content = correctedText });

            // Act
            var result = await service.ProcessTextAsync(input);

            // Assert
            //Assert.Equal(correctedText, result);
            _output.WriteLine($"Input : {input}");
            _output.WriteLine($"Output : {result.correctedText}");
            _output.WriteLine($"Diff : {result.diffText}");

            Assert.NotEmpty(result.correctedText);
        }

        [Fact]
        public async Task ProcessTextAsync_ReturnsCorrected_llama31_Text()
        {
            // Arrange
            var input = inputText;
            _configurationMock.Setup(c => c["ModelSettings:ModelName"]).Returns("Ollama:llama3.1");
            _configurationMock.Setup(c => c["ModelSettings:Address"]).Returns("http://localhost:11434");

            var service = new OrthoService(_loggerMock.Object, _httpClientFactoryMock.Object, _configurationMock.Object);

            //_chatServiceMock.Setup(cs => cs.GetChatMessageContentAsync(It.IsAny<ChatHistory>()))
            //    .ReturnsAsync(new ChatMessage { Content = correctedText });

            // Act
            var result = await service.ProcessTextAsync(input);

            // Assert
            //Assert.Equal(correctedText, result);
            _output.WriteLine($"Input : {input}");
            _output.WriteLine($"Output : {result.correctedText}");
            _output.WriteLine($"Diff : {result.diffText}");

            Assert.NotEmpty(result.correctedText);
        }


        [Fact]
        public async Task ProcessTextAsync_ReturnsCorrected_llama3_Text()
        {
            // Arrange
            var input = inputText;
            _configurationMock.Setup(c => c["ModelSettings:ModelName"]).Returns("Ollama:llama3");
            _configurationMock.Setup(c => c["ModelSettings:Address"]).Returns("http://localhost:11434");

            var service = new OrthoService(_loggerMock.Object, _httpClientFactoryMock.Object, _configurationMock.Object);

            //_chatServiceMock.Setup(cs => cs.GetChatMessageContentAsync(It.IsAny<ChatHistory>()))
            //    .ReturnsAsync(new ChatMessage { Content = correctedText });

            // Act
            var result = await service.ProcessTextAsync(input);

            // Assert
            //Assert.Equal(correctedText, result);
            _output.WriteLine($"Input : {input}");
            _output.WriteLine($"Output : {result.correctedText}");
            _output.WriteLine($"Diff : {result.diffText}");

            Assert.NotEmpty(result.correctedText);
        }

        [Fact]
        public async Task ProcessTextAsync_ReturnsCorrected_mistral_Text()
        {
            // Arrange
            var input = inputText;
            _configurationMock.Setup(c => c["ModelSettings:ModelName"]).Returns("mistral");
            _configurationMock.Setup(c => c["ModelSettings:Address"]).Returns("http://localhost:11434");

            var service = new OrthoService(_loggerMock.Object, _httpClientFactoryMock.Object, _configurationMock.Object);

            //_chatServiceMock.Setup(cs => cs.GetChatMessageContentAsync(It.IsAny<ChatHistory>()))
            //    .ReturnsAsync(new ChatMessage { Content = correctedText });

            // Act
            var result = await service.ProcessTextAsync(input);

            // Assert
            //Assert.Equal(correctedText, result);
            _output.WriteLine($"Input : {input}");
            _output.WriteLine($"Output : {result.correctedText}");
            _output.WriteLine($"Diff : {result.diffText}");

            Assert.NotEmpty(result.correctedText);
        }
       
        //[Fact]
        //public async Task ProcessTextAsync_ReturnsCorrected_deepseekR115_Text()
        //{
        //    // Arrange
        //    var input = inputText;
        //    _configurationMock.Setup(c => c["ModelSettings:ModelName"]).Returns("deepseek-r1:1.5b");
        //    _configurationMock.Setup(c => c["ModelSettings:Address"]).Returns("http://localhost:11434");

        //    var service = new OrthoService(_loggerMock.Object, _httpClientFactoryMock.Object, _configurationMock.Object);

        //    //_chatServiceMock.Setup(cs => cs.GetChatMessageContentAsync(It.IsAny<ChatHistory>()))
        //    //    .ReturnsAsync(new ChatMessage { Content = correctedText });

        //    // Act
        //    var result = await service.ProcessTextAsync(input);

        //    // Assert
        //    //Assert.Equal(correctedText, result);
        //    _output.WriteLine($"Input : {input}");
        //    _output.WriteLine($"Output : {result.correctedText}");
        //    _output.WriteLine($"Diff : {result.diffText}");

        //    Assert.NotEmpty(result.correctedText);
        //}

        //[Fact]
        //public async Task ProcessTextAsync_ReturnsCorrected_deepseekR1_Text()
        //{
        //    // Arrange
        //    var input = inputText;
        //    _configurationMock.Setup(c => c["ModelSettings:ModelName"]).Returns("deepseek-r1");
        //    _configurationMock.Setup(c => c["ModelSettings:Address"]).Returns("http://localhost:11434");

        //    var service = new OrthoService(_loggerMock.Object, _httpClientFactoryMock.Object, _configurationMock.Object);

        //    //_chatServiceMock.Setup(cs => cs.GetChatMessageContentAsync(It.IsAny<ChatHistory>()))
        //    //    .ReturnsAsync(new ChatMessage { Content = correctedText });

        //    // Act
        //    var result = await service.ProcessTextAsync(input);

        //    // Assert
        //    //Assert.Equal(correctedText, result);
        //    _output.WriteLine($"Input : {input}");
        //    _output.WriteLine($"Output : {result.correctedText}");
        //    _output.WriteLine($"Diff : {result.diffText}");

        //    Assert.NotEmpty(result.correctedText);
        //    Assert.DoesNotContain("Erreur lors du traitement", result.correctedText);
        //}
        //[Fact]
        //public async Task ProcessTextAsync_ReturnsCorrected_deepseekR114b_Text()
        //{
        //    // Arrange
        //    var input = inputText;
        //    _configurationMock.Setup(c => c["ModelSettings:ModelName"]).Returns("deepseek-r1:14b");
        //    _configurationMock.Setup(c => c["ModelSettings:Address"]).Returns("http://localhost:11434");

        //    var service = new OrthoService(_loggerMock.Object, _httpClientFactoryMock.Object, _configurationMock.Object);

        //    //_chatServiceMock.Setup(cs => cs.GetChatMessageContentAsync(It.IsAny<ChatHistory>()))
        //    //    .ReturnsAsync(new ChatMessage { Content = correctedText });

        //    // Act
        //    var result = await service.ProcessTextAsync(input);

        //    // Assert
        //    //Assert.Equal(correctedText, result);
        //    _output.WriteLine($"Input : {input}");
        //    _output.WriteLine($"Output : {result.correctedText}");
        //    _output.WriteLine($"Diff : {result.diffText}");

        //    Assert.NotEmpty(result.correctedText);
        //    Assert.DoesNotContain("Erreur lors du traitement", result.correctedText);
        //}



        [Fact]
        public async Task ProcessTextAsync_ReturnsCorrected_gemma3_Text()
        {
            // Arrange
            var input = inputText;
            _configurationMock.Setup(c => c["ModelSettings:ModelName"]).Returns("Ollama:gemma3");
            _configurationMock.Setup(c => c["ModelSettings:Address"]).Returns("http://localhost:11434");

            var service = new OrthoService(_loggerMock.Object, _httpClientFactoryMock.Object, _configurationMock.Object);

            //_chatServiceMock.Setup(cs => cs.GetChatMessageContentAsync(It.IsAny<ChatHistory>()))
            //    .ReturnsAsync(new ChatMessage { Content = correctedText });

            // Act
            var result = await service.ProcessTextAsync(input);

            // Assert
            //Assert.Equal(correctedText, result);
            _output.WriteLine($"Input : {input}");
            _output.WriteLine($"Output : {result.correctedText}");
            _output.WriteLine($"Diff : {result.diffText}");

            Assert.NotEmpty(result.correctedText);
            Assert.DoesNotContain("Erreur lors du traitement", result.correctedText);
        }

        [Fact]
        public async Task ProcessTextAsync_ReturnsCorrected_gemma312_Text()
        {
            // Arrange
            var input = inputText;
            _configurationMock.Setup(c => c["ModelSettings:ModelName"]).Returns("Ollama:gemma3:12b");
            _configurationMock.Setup(c => c["ModelSettings:Address"]).Returns("http://localhost:11434");

            var service = new OrthoService(_loggerMock.Object, _httpClientFactoryMock.Object, _configurationMock.Object);

            //_chatServiceMock.Setup(cs => cs.GetChatMessageContentAsync(It.IsAny<ChatHistory>()))
            //    .ReturnsAsync(new ChatMessage { Content = correctedText });

            // Act
            var result = await service.ProcessTextAsync(input);

            // Assert
            //Assert.Equal(correctedText, result);
            _output.WriteLine($"Input : {input}");
            _output.WriteLine($"Output : {result.correctedText}");
            _output.WriteLine($"Diff : {result.diffText}");

            Assert.NotEmpty(result.correctedText);
            Assert.DoesNotContain("Erreur lors du traitement", result.correctedText);
        }


        //[Fact]
        //public async Task ProcessTextAsync_ReturnsCorrected_apxObsidian_Text()
        //{
        //    // Arrange
        //    var input = inputText;
        //    _configurationMock.Setup(c => c["ModelSettings:ModelName"]).Returns("apx/obsidian:fr");
        //    _configurationMock.Setup(c => c["ModelSettings:Address"]).Returns("http://localhost:11434");

        //    var service = new OrthoService(_loggerMock.Object, _httpClientFactoryMock.Object, _configurationMock.Object);

        //    //_chatServiceMock.Setup(cs => cs.GetChatMessageContentAsync(It.IsAny<ChatHistory>()))
        //    //    .ReturnsAsync(new ChatMessage { Content = correctedText });

        //    // Act
        //    var result = await service.ProcessTextAsync(input);

        //    // Assert
        //    //Assert.Equal(correctedText, result);
        //    _output.WriteLine($"Input : {input}");
        //    _output.WriteLine($"Output : {result.correctedText}");
        //    _output.WriteLine($"Diff : {result.diffText}");

        //    Assert.NotEmpty(result.correctedText);
        //    Assert.DoesNotContain("Erreur lors du traitement", result.correctedText);
        //}


        //[Fact]
        //public async Task ProcessTextAsync_LogsErrorOnException()
        //{
        //    // Arrange
        //    _configurationMock.Setup(c => c["ModelSettings:ModelName"]).Returns("llama3.2");
        //    _configurationMock.Setup(c => c["ModelSettings:Address"]).Returns("http://localhost:11434");

        //    var service = new OrthoService(_loggerMock.Object, _httpClientFactoryMock.Object, _configurationMock.Object);

        //    //_chatServiceMock.Setup(cs => cs.GetChatMessageContentAsync(It.IsAny<ChatHistory>()))
        //    //    .ThrowsAsync(new Exception("Test exception"));

        //    // Act
        //    var result = await service.ProcessTextAsync("Original text");

        //    // Assert
        //    Assert.Contains("Erreur lors du traitement", result.correctedText);
        //    _loggerMock.Verify(l => l.LogError(It.IsAny<Exception>(), "Erreur lors du traitement du texte"), Times.Once);
        //}

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

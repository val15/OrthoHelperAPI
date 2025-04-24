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
        public async Task ProcessTextAsync_longText_ReturnsCorrected_gemma3_Text()
        {
            // Arrange
            var input = "1 : Gestion de coupur internet traitement des ticker : user->anager et domoina -> relancer infini : avoir des retour\r\n\t\tProbeme courant,…\r\n\t\tAPI pour suivre les Ticket ou on suis ça sur le web ?\r\n\t\tDB lite, repporting\r\n\t\tFront qui ckeck les tikets\r\n\t\tBack pour stoquer le donnéer\r\n\t\tRepportint en tableau, power be,…\r\n\t2 : situation internet: test de connection, carte, geolocalisation, nperf\r\n\t\tClient qui vars aller sur nperf,\r\n\t\tIl faut rensaigner la localisation (dans un premier temps, pas de géolocalisation)\r\n\t\tAvoir une interface coté admin pour la visibilitée avoir une carte par example ou dans un premier temps un tableau par zone pour par utilisateur\r\n\t\tUn DB costo avec un système de repporting\r\n\t\tFront js (reactc, angular,..)\r\n\t\tBack c#, java\r\n\t3 : portal novity : othontification, droits, info, newslattrer :\r\n\t\tDashBoar : c’est une interface qui contien toute les lien de novity avec un système de motification\r\n\t\tFaciliter d’ajouter et de suplimiler les éléménts\r\n\t\tWEB front : PHP, angula,react ou simple HTLM\r\n\t4 : platforme de mise en relation stagiere des écoles et les sociétes :\r\n\t\tBd fort,\r\n\t\tMicroserveice\r\n\t\tCapaciter de scale (geberger sur des clause et accessible via des portale web qui scalenet)\r\n\t\tBack c#,java, ….\r\n\t\tFront angulal ,etc\r\n\r\n5 tous doit etre en micro service\r\net doit étre portable pour etre heberger n'import'où\r\n\r\n";

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
        public async Task AskQuestionForIaAsync_gemini20()
        {
            // Arrange
            var input = "QFIA Qui es tu?";

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
        public async Task AskQuestionForIaAsync_longText_LongText_gemini20()
        {
            // Arrange
            var input = "QFIA Peut tu corriger : 1 : Gestion de coupur internet traitement des ticker : user->anager et domoina -> relancer infini : avoir des retour\r\n\t\tProbeme courant,…\r\n\t\tAPI pour suivre les Ticket ou on suis ça sur le web ?\r\n\t\tDB lite, repporting\r\n\t\tFront qui ckeck les tikets\r\n\t\tBack pour stoquer le donnéer\r\n\t\tRepportint en tableau, power be,…\r\n\t2 : situation internet: test de connection, carte, geolocalisation, nperf\r\n\t\tClient qui vars aller sur nperf,\r\n\t\tIl faut rensaigner la localisation (dans un premier temps, pas de géolocalisation)\r\n\t\tAvoir une interface coté admin pour la visibilitée avoir une carte par example ou dans un premier temps un tableau par zone pour par utilisateur\r\n\t\tUn DB costo avec un système de repporting\r\n\t\tFront js (reactc, angular,..)\r\n\t\tBack c#, java\r\n\t3 : portal novity : othontification, droits, info, newslattrer :\r\n\t\tDashBoar : c’est une interface qui contien toute les lien de novity avec un système de motification\r\n\t\tFaciliter d’ajouter et de suplimiler les éléménts\r\n\t\tWEB front : PHP, angula,react ou simple HTLM\r\n\t4 : platforme de mise en relation stagiere des écoles et les sociétes :\r\n\t\tBd fort,\r\n\t\tMicroserveice\r\n\t\tCapaciter de scale (geberger sur des clause et accessible via des portale web qui scalenet)\r\n\t\tBack c#,java, ….\r\n\t\tFront angulal ,etc\r\n\r\n5 tous doit etre en micro service\r\net doit étre portable pour etre heberger n'import'où\r\n\r\n ?";

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
            Assert.Contains("5", result);
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
        public async Task ProcessTextAsync_longText_ReturnsCorrected_gemini20_Text()
        {
            // Arrange
            var input = "1 : Gestion de coupur internet traitement des ticker : user->anager et domoina -> relancer infini : avoir des retour\r\n\t\tProbeme courant,…\r\n\t\tAPI pour suivre les Ticket ou on suis ça sur le web ?\r\n\t\tDB lite, repporting\r\n\t\tFront qui ckeck les tikets\r\n\t\tBack pour stoquer le donnéer\r\n\t\tRepportint en tableau, power be,…\r\n\t2 : situation internet: test de connection, carte, geolocalisation, nperf\r\n\t\tClient qui vars aller sur nperf,\r\n\t\tIl faut rensaigner la localisation (dans un premier temps, pas de géolocalisation)\r\n\t\tAvoir une interface coté admin pour la visibilitée avoir une carte par example ou dans un premier temps un tableau par zone pour par utilisateur\r\n\t\tUn DB costo avec un système de repporting\r\n\t\tFront js (reactc, angular,..)\r\n\t\tBack c#, java\r\n\t3 : portal novity : othontification, droits, info, newslattrer :\r\n\t\tDashBoar : c’est une interface qui contien toute les lien de novity avec un système de motification\r\n\t\tFaciliter d’ajouter et de suplimiler les éléménts\r\n\t\tWEB front : PHP, angula,react ou simple HTLM\r\n\t4 : platforme de mise en relation stagiere des écoles et les sociétes :\r\n\t\tBd fort,\r\n\t\tMicroserveice\r\n\t\tCapaciter de scale (geberger sur des clause et accessible via des portale web qui scalenet)\r\n\t\tBack c#,java, ….\r\n\t\tFront angulal ,etc\r\n\r\n5 tous doit etre en micro service\r\net doit étre portable pour etre heberger n'import'où\r\n\r\n";

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
            Assert.Contains("5", result);
            Assert.DoesNotContain("Erreur lors du traitement", result);
        }


        [Fact]
        public async Task Split_longText_ReturnsCorrectPartNumber()
        {
            // Arrange
            var input = "1 : Gestion de coupur internet traitement des ticker : user->anager et domoina -> relancer infini : avoir des retour\r\n\t\tProbeme courant,…\r\n\t\tAPI pour suivre les Ticket ou on suis ça sur le web ?\r\n\t\tDB lite, repporting\r\n\t\tFront qui ckeck les tikets\r\n\t\tBack pour stoquer le donnéer\r\n\t\tRepportint en tableau, power be,…\r\n\t2 : situation internet: test de connection, carte, geolocalisation, nperf\r\n\t\tClient qui vars aller sur nperf,\r\n\t\tIl faut rensaigner la localisation (dans un premier temps, pas de géolocalisation)\r\n\t\tAvoir une interface coté admin pour la visibilitée avoir une carte par example ou dans un premier temps un tableau par zone pour par utilisateur\r\n\t\tUn DB costo avec un système de repporting\r\n\t\tFront js (reactc, angular,..)\r\n\t\tBack c#, java\r\n\t3 : portal novity : othontification, droits, info, newslattrer :\r\n\t\tDashBoar : c’est une interface qui contien toute les lien de novity avec un système de motification\r\n\t\tFaciliter d’ajouter et de suplimiler les éléménts\r\n\t\tWEB front : PHP, angula,react ou simple HTLM\r\n\t4 : platforme de mise en relation stagiere des écoles et les sociétes :\r\n\t\tBd fort,\r\n\t\tMicroserveice\r\n\t\tCapaciter de scale (geberger sur des clause et accessible via des portale web qui scalenet)\r\n\t\tBack c#,java, ….\r\n\t\tFront angulal ,etc\r\n\r\n5 tous doit etre en micro service\r\net doit étre portable pour etre heberger n'import'où\r\n\r\n";

            //var engine = new OrthoEngine(_client, _mockRepo.Object, _mockCurrentUserService.Object, _mockLogger.Object);
            //engine.ModelName = "Online:gemini-2.0-flash";
            ////_chatServiceMock.Setup(cs => cs.GetChatMessageContentAsync(It.IsAny<ChatHistory>()))
            ////    .ReturnsAsync(new ChatMessage { Content = correctedText });


            // Act
            var testPart = TextHelper.DivideTextIntoParts(input, 1000);

            // Assert
            Assert.Equal(testPart.Count, 2);
        }

        [Fact]
        public async Task Split_shortText_ReturnsCorrectPartNumber()
        {
            // Arrange
            var input = "Gestion de coupur internet traitement des ticker ";

            //var engine = new OrthoEngine(_client, _mockRepo.Object, _mockCurrentUserService.Object, _mockLogger.Object);
            //engine.ModelName = "Online:gemini-2.0-flash";
            ////_chatServiceMock.Setup(cs => cs.GetChatMessageContentAsync(It.IsAny<ChatHistory>()))
            ////    .ReturnsAsync(new ChatMessage { Content = correctedText });


            // Act
            var testPart = TextHelper.DivideTextIntoParts(input, 1000);

            // Assert
            Assert.Equal(testPart.Count, 1);
        }




        [Fact]
        public void TestCharacterDifference()
        {
            // Arrange
            string text1 = inputText;
            string text2 = "Salut Henitsoa, je viens vous aider à la demande de Hafalina pour donner mon coup de main sur l'application NovyParking que vous développez.";

            // Act
            string result = TextHelper.GenerateCharacterDiff(text1, text2);

            // Assert
            _output.WriteLine("Différences HTML générées :");
            _output.WriteLine(result);

            Assert.Contains("text-decoration: line-through", result);
            Assert.Contains("color: blue", result);
        }
    }
}

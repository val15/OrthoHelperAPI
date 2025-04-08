using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.SemanticKernel.ChatCompletion;
using OllamaSharp;
using OrthoHelperAPI.Services.Interfaces;
using System.Diagnostics;
using Tools;

namespace OrthoHelperAPI.Services
{



    public class OrthoService : ITextProcessingService
    {
        private readonly IChatCompletionService _chatService;
        private readonly ILogger<OrthoService> _logger;
        private readonly string _modelName; //a mettre dans le appsettings.json et pous tard dans les sercet ou varible d'environnement
        private readonly string _ollamaAdress;//a mettre dans le appsettings.json et pous tard dans les sercet ou varible d'environnement
        private readonly ChatHistory _history;

        public OrthoService(ILogger<OrthoService> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            try
            {
                _logger = logger;
                _modelName = configuration["ModelSettings:ModelName"];
                if (_modelName.Contains("Ollama"))
                {
                    _ollamaAdress = configuration["ModelSettings:Address"];
                    _modelName = _modelName.Replace("Ollama:","");


                    var httpClient = new HttpClient
                    {
                        BaseAddress = new Uri(_ollamaAdress),
                        Timeout = TimeSpan.FromMinutes(15),
                    };
#pragma warning disable SKEXP0001
                    // _chatService = new OllamaApiClient(httpClient).AsChatCompletionService();
                    _chatService = new OllamaApiClient(httpClient, _modelName).AsChatCompletionService();

#pragma warning restore SKEXP0001
                }
                //else if (_modelName.Contains("Online"))
                //{
                //    var httpClient = new HttpClient
                //    {
                //        BaseAddress = new Uri(_ollamaAdress),
                //        Timeout = TimeSpan.FromMinutes(15),
                //    };

                //}

                    _history = new();
                //_history.AddSystemMessage("Tu es une IA spécialiste en correction d'orthographe. À chaque texte que tu reçois, retourne le texte corrigé sans rien d'autre.");
                var initText = "";
                //initText = "Tu es une IA spécialiste en correction d'orthographe, et de grammaire.";

                initText = "Tu es un assistant pour mon éditeur de texte Obsidian. Tu me parles exclusivement en français même si je te parle dans une autre langue, tu me réponds en français. \r\nTu dois pouvoir me corriger mes fautes d'orthographe ou reformuler mes phrases si elles sont grammaticalement incorrectes. Si le text est du markdown, tu ne dois pas le modifier.";


                _history.AddSystemMessage(initText);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'initialisation du service Ollama");
                throw;
            }
        }

        public async Task<(string correctedText, string diffText)> ProcessTextAsync(string text, List<OrthoHelperAPI.Model.Message> messagesList = null)
        {
            try
            {

                //Debug.WriteLine(_modelName);
                //Console.WriteLine(_modelName);

                //add old messages
                if (messagesList != null)
                {
                    foreach (var message in messagesList)
                    {
                        _history.AddUserMessage(message.InputText.Replace("QFIA", ""));

                        _history.AddAssistantMessage(message.OutputText);
                        _history.AddUserMessage(message.Diff);
                    }
                }


                // Ici, vous pouvez implémenter l'appel à Ollama
                //QUESTION FOR IA
                if (text.Contains("QFIA"))
                    _history.AddUserMessage(text.Replace("QFIA", ""));
                else
                    _history.AddUserMessage($"Peux-tu corriger ? (retourne seulement le texte corrigé) : {text}");

                // _history.AddRange(messagesList);

                var result = await _chatService.GetChatMessageContentAsync(_history);
                Debug.WriteLine(result.Content);
                Console.WriteLine(result.Content);

                _history.Add(result);

                var diff = TextDiffHelper.GenerateCharacterDiff(text, result.Content);
                _history.AddUserMessage($"differences : {diff}");

                // Retourne un tuple nommé
                return (result.Content, diff);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du traitement du texte");
                return ("Erreur lors du traitement : " + text, "");
            }
        }


    }
}

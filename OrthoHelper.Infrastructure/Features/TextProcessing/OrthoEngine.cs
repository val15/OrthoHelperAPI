using System.Diagnostics;
using System.Net.Http;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;
using OllamaSharp;
using OrthoHelper.Domain.Features.Common.Ports;
using OrthoHelper.Domain.Features.TextCorrection.Ports.Repositories;
using Tools;

namespace OrthoHelper.Infrastructure.Features.TextProcessing
{



    public class OrthoEngine : IOrthoEngine
    {
        private  IChatCompletionService _chatService;
        private  ChatHistory _history;
        private readonly ICorrectionSessionRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        private readonly HttpClient _httpClient;

        private string _modelName = "Ollama:Gemma";

        public string ModelName
        {
            get => _modelName;
            set => _modelName = value;
        }

        public void SetModelName(string modelName)
        {
            _modelName = modelName;
        }


        //  private readonly ILogger<OrthoService> _logger;
        public OrthoEngine(HttpClient httpClient, ICorrectionSessionRepository repository, ICurrentUserService currentUserService)
        {

            //_logger = logger;
            //_modelName = configuration["ModelSettings:ModelName"];
            //_ollamaAdress = configuration["ModelSettings:Address"];


            try
            {
                _currentUserService = currentUserService;
                _repository = repository;
                _httpClient = httpClient;
                //InitializeByModelName();

               

            }
            catch (Exception ex)
            {
                //  _logger.LogError(ex, "Erreur lors de l'initialisation du service Ollama");
                throw;
            }
        }

        public void InitializeByModelName()
        {
            var realModelName= string.Empty;
            if (_modelName.Contains("Ollama"))
            {
                realModelName = _modelName.Replace("Ollama:", "");
#pragma warning disable SKEXP0001
                // _chatService = new OllamaApiClient(httpClient).AsChatCompletionService();
                _chatService = new OllamaApiClient(_httpClient, realModelName).AsChatCompletionService();

#pragma warning restore SKEXP0001
            }
            else if (_modelName.Contains("Online"))
            {
                realModelName = _modelName.Replace("Online:", "");
                if (_modelName.Contains("gemini"))
                {

                    var apiKeyGemini = Environment.GetEnvironmentVariable("GOOGLE_AI_GEMINI_API_KEY");
#pragma warning disable SKEXP0070
                    // Utilisation de la classe expérimentale
                    _chatService = new GoogleAIGeminiChatCompletionService(
                        modelId: realModelName,//"gemini-2.0-flash",
                        apiKey: apiKeyGemini
                    );
#pragma warning restore SKEXP0070
                }
            }

            _history = new();
            //_history.AddSystemMessage("Tu es une IA spécialiste en correction d'orthographe. À chaque texte que tu reçois, retourne le texte corrigé sans rien d'autre.");
            var initText = "";
            //initText = "Tu es une IA spécialiste en correction d'orthographe, et de grammaire.";

            initText = "Tu es un assistant pour mon éditeur de texte Obsidian. Tu me parles exclusivement en français même si je te parle dans une autre langue, tu me réponds en français. \r\nTu dois pouvoir me corriger mes fautes d'orthographe ou reformuler mes phrases si elles sont grammaticalement incorrectes. Si le text est du markdown, tu ne dois pas le modifier.";



            var historyText = "";
            // Appel de InitializeUserSession
            if (_currentUserService.IsAuthenticated)
            {
                historyText = GetTextUserSessionHistory().Result;
            }
            //TODO AJOUTER EXPLICATIONS
            _history.AddSystemMessage(initText + "\nVoici l'historique de nos échanges : \n " + historyText);

        }


        public async Task InitializeUserSession(string username)
        {
            var sessions = await _repository.GetCorrectionSessionsAsync();

            foreach (var session in sessions)
            {
                _history.AddUserMessage(session.OriginalText);
                _history.AddAssistantMessage(session.CorrectedText);
            }
        }

        public async Task<string> GetTextUserSessionHistory()
        {
            var output = "";
            var sessions = await _repository.GetCorrectionSessionsAsync();

            foreach (var session in sessions)
            {
                output += $"user message : {session.OriginalText} .\n";
                output += $"your response : {session.OriginalText} .\n";
                output += $"diff: {session.Diff} .\n";

            }
            return output;
        }

        // Avant (synchrone)
        // public string ProcessText(string inputText) { ... }

        // Après (asynchrone)
        public async Task<string> ProcessTextAsync(string inputText)
        {
            // Exemple de logique synchrone wrappée dans une Task
            try
            {
                InitializeByModelName();

                //add old messages
                //if (messagesList != null)
                //{
                //    foreach (var message in messagesList)
                //    {
                //        _history.AddUserMessage(message.InputText.Replace("QFIA", ""));

                //        _history.AddAssistantMessage(message.OutputText);
                //        _history.AddUserMessage(message.Diff);
                //    }
                //}


                // Ici, vous pouvez implémenter l'appel à Ollama
                //QUESTION FOR IA
                if (inputText.Contains("QFIA"))
                    _history.AddUserMessage(inputText.Replace("QFIA", ""));
                else
                    _history.AddUserMessage($"Peux-tu corriger ? (retourne seulement le texte corrigé) : {inputText}");

                // _history.AddRange(messagesList);

                var result = await _chatService.GetChatMessageContentAsync(_history);
                var textResult = result.Content.Trim();
                Debug.WriteLine(textResult);
                Console.WriteLine(textResult);
                _history.Add(result);

                var diff = TextDiffHelper.GenerateCharacterDiff(inputText, result.Content.Trim());
                _history.AddUserMessage($"differences : {diff}");

                // Retourne un tuple nommé
                // return (result.Content, diff);
                return textResult;
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, "Erreur lors du traitement du texte");
                // return ("Erreur lors du traitement : " + text, "");
                return "Erreur lors du traitement : " + inputText;
            }
        }
    }
}

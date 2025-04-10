using System.Diagnostics;
using System.Net.Http;
using Microsoft.Extensions.Logging; // Ajoutez cet using
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
        private IChatCompletionService _chatService;
        private ChatHistory _history;
        private readonly ICorrectionSessionRepository _repository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<OrthoEngine> _logger; // Ajoutez le logger

        private readonly HttpClient _httpClient;

        private string _modelName = "Ollama:Gemma3";

        public string ModelName
        {
            get => _modelName;
            set => _modelName = value;
        }

        public void SetModelName(string modelName)
        {
            _modelName = modelName;
        }

        // Constructeur mis à jour pour accepter ILogger
        public OrthoEngine(HttpClient httpClient, ICorrectionSessionRepository repository, ICurrentUserService currentUserService, ILogger<OrthoEngine> logger)
        {
            _currentUserService = currentUserService;
            _repository = repository;
            _httpClient = httpClient;
            _logger = logger; // Assignez l'instance du logger

            try
            {
                // InitializeByModelName(); // Peut être appelé plus tard si nécessaire
                _logger.LogInformation("OrthoEngine initialisé.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'initialisation du service OrthoEngine.");
                throw;
            }
        }

        public void InitializeByModelName()
        {
            try
            {
                var realModelName = string.Empty;
                _logger.LogInformation($"Tentative d'initialisation du modèle: {_modelName}");
                if (_modelName.Contains("Ollama"))
                {
                    realModelName = _modelName.Replace("Ollama:", "");
#pragma warning disable SKEXP0001
                    _chatService = new OllamaApiClient(_httpClient, realModelName).AsChatCompletionService();
                    _logger.LogDebug($"Service Ollama initialisé avec le modèle: {realModelName}");
#pragma warning restore SKEXP0001
                }
                else if (_modelName.Contains("Online"))
                {
                    realModelName = _modelName.Replace("Online:", "");
                    if (_modelName.Contains("gemini"))
                    {
                        var apiKeyGemini = Environment.GetEnvironmentVariable("GOOGLE_AI_GEMINI_API_KEY");
                        if (apiKeyGemini == null)
                        {
                            _logger.LogError("La clé API Gemini est nulle.");
                            Console.WriteLine("apiKeyGemini IS NULL"); // Gardez ceci temporairement pour le débogage initial dans Docker
                        }
#pragma warning disable SKEXP0070
                        _chatService = new GoogleAIGeminiChatCompletionService(
                            modelId: realModelName,
                            apiKey: apiKeyGemini
                        );
                        _logger.LogDebug($"Service Google Gemini initialisé avec le modèle: {realModelName}");
#pragma warning restore SKEXP0070
                    }
                    else
                    {
                        _logger.LogWarning($"Type de modèle en ligne non géré: {_modelName}");
                    }
                }
                else
                {
                    _logger.LogWarning($"Type de modèle non géré: {_modelName}");
                }

                _history = new();
                var initText = "Tu es un assistant pour mon éditeur de texte Obsidian. Tu me parles exclusivement en français même si je te parle dans une autre langue, tu me réponds en français. \r\nTu dois pouvoir me corriger mes fautes d'orthographe ou reformuler mes phrases si elles sont grammaticalement incorrectes. Si le text est du markdown, tu ne dois pas le modifier.";

                var historyText = "";
                if (_currentUserService.IsAuthenticated)
                {
                    historyText = GetTextUserSessionHistory().Result;
                    _logger.LogDebug($"Historique de la session utilisateur récupéré: {historyText}");
                }
                _history.AddSystemMessage(initText + "\nVoici l'historique de nos échanges : \n " + historyText);
                _logger.LogDebug($"Message système ajouté à l'historique: {initText}");
            }
            catch (Exception ex)
            {

                _logger.LogError($"Exeption : {ex}");
            }
            
        }

        public async Task InitializeUserSession(string username)
        {
            _logger.LogInformation($"Initialisation de la session utilisateur pour: {username}");
            var sessions = await _repository.GetCorrectionSessionsAsync();
            _logger.LogDebug($"Nombre de sessions de correction récupérées: {sessions.Count()}");

            foreach (var session in sessions)
            {
                _history.AddUserMessage(session.OriginalText);
                _history.AddAssistantMessage(session.CorrectedText);
                _logger.LogTrace($"Message utilisateur ajouté à l'historique: {session.OriginalText}");
                _logger.LogTrace($"Réponse de l'assistant ajoutée à l'historique: {session.CorrectedText}");
            }
        }

        public async Task<string> GetTextUserSessionHistory()
        {
            _logger.LogInformation("Récupération de l'historique de la session utilisateur sous forme de texte.");
            var output = "";
            var sessions = await _repository.GetCorrectionSessionsAsync();
            _logger.LogDebug($"Nombre de sessions de correction récupérées pour l'historique: {sessions.Count()}");

            foreach (var session in sessions)
            {
                output += $"user message : {session.OriginalText} .\n";
                output += $"your response : {session.CorrectedText} .\n"; // Correction ici
                output += $"diff: {session.Diff} .\n";
                _logger.LogTrace($"Ligne d'historique ajoutée: Utilisateur - {session.OriginalText}, Assistant - {session.CorrectedText}, Diff - {session.Diff}");
            }
            _logger.LogDebug($"Historique de la session utilisateur récupéré: {output}");
            return output;
        }

        public async Task<string> ProcessTextAsync(string inputText)
        {
            _logger.LogInformation($"Traitement du texte entrant: {inputText}");
            try
            {
                InitializeByModelName();

                if (inputText.Contains("QFIA"))
                {
                    var userMessage = inputText.Replace("QFIA", "");
                    _history.AddUserMessage(userMessage);
                    _logger.LogDebug($"Message utilisateur (QFIA) ajouté à l'historique: {userMessage}");
                }
                else
                {
                    var userMessage = $"Peux-tu corriger ? (retourne seulement le texte corrigé) : {inputText}";
                    _history.AddUserMessage(userMessage);
                    _logger.LogDebug($"Message utilisateur (correction) ajouté à l'historique: {userMessage}");
                }

                _logger.LogInformation("Appel du service de complétion de chat.");
                var result = await _chatService.GetChatMessageContentAsync(_history);
                var textResult = result.Content?.Trim();
                _logger.LogInformation($"Réponse du service de complétion: {textResult}");
                Debug.WriteLine(textResult);
                Console.WriteLine(textResult); // Utile pour le débogage initial dans Docker
                if (result != null)
                {
                    _history.Add(result);
                }

                if (textResult != null)
                {
                    var diff = TextDiffHelper.GenerateCharacterDiff(inputText, textResult);
                    _history.AddUserMessage($"differences : {diff}");
                    _logger.LogDebug($"Différences générées: {diff}");
                    return textResult;
                }
                else
                {
                    _logger.LogWarning("La réponse du service de complétion est nulle.");
                    return "Erreur lors du traitement : Réponse du service nulle.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors du traitement du texte: {inputText}");
                return $"Erreur lors du traitement : {inputText}";
            }
        }
    }
}
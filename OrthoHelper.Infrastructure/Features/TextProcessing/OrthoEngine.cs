using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;
using OllamaSharp;
using OrthoHelper.Domain.Features.Common.Ports;
using OrthoHelper.Domain.Features.TextCorrection.Entities;
using OrthoHelper.Domain.Features.TextCorrection.Ports;
using OrthoHelper.Domain.Features.TextCorrection.Ports.Repositories;
using System.Diagnostics;
using Tools;
using static OrthoHelper.Domain.Features.TextCorrection.Entities.Session;

namespace OrthoHelper.Infrastructure.Features.TextProcessing
{
    public abstract class OrthoEngine : IOrthoEngine
    {
        protected IChatCompletionService _chatService;
        protected ChatHistory _history;

        protected readonly ISessionRepository _sessionRepository;
        protected readonly ICurrentUserService _currentUserService;
        protected readonly ILogger<OrthoEngine> _logger;

        protected readonly HttpClient _httpClient;

        protected readonly int _maxCharsPerPartGemmini = 1000;
        protected bool _subdiviseText = false;

        protected string _modelName = "Ollama:Gemma3";
        protected int _minDelayTime = 7000;
        protected int _retyCount = 1;

        protected string _className; 

        public abstract string InitText { get; }
        public abstract string BottomOfThequestion { get; }
        private bool _isInited=false;
        public string ModelName
        {
            get => _modelName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Le nom du modèle ne peut pas être vide.", nameof(value));

                _modelName = value;
                InitializeUsingModelName();
            }
        }

        public void SetModelName(string modelName)
        {
           ModelName = modelName;
        }


        public string GetModelName()
        {
            return _modelName;
        }
        public OrthoEngine(HttpClient client, 
            ISessionRepository correctionSessionRepository,
            ICurrentUserService currentUserService, 
            ILogger<OrthoEngine> logger)
        {
            _httpClient = client;
            _sessionRepository = correctionSessionRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public  void InitializeUsingModelName()
        {
            try
            {
                if(_isInited)
                    return;

                _history = new();
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
                        _subdiviseText = true;
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

               

                var historyText = "";
                if (_currentUserService.IsAuthenticated)
                {
                    historyText = GetTextUserSessionHistory().Result;
                    _logger.LogDebug($"Historique de la session utilisateur récupéré: {historyText}");
                }
                _history.AddSystemMessage(InitText + "\nVoici l'historique de nos échanges : \n " + historyText);
                _logger.LogDebug($"Message système ajouté à l'historique: {InitText}");
                _isInited = true;
            }
            catch (Exception ex)
            {

                _logger.LogError($"Exeption : {ex}");
                _isInited = false;
            }

        }
        public async Task<string> GetTextUserSessionHistory()
        {
            try
            {
                _logger.LogInformation("Récupération de l'historique de la session utilisateur sous forme de texte.");
                var output = "";
                var sessions = await _sessionRepository.GetSessionsAsync();
                _logger.LogDebug($"Nombre de sessions de correction récupérées pour l'historique: {sessions.Count()}");

                foreach (var session in sessions)
                {
                    output += $"user message : {session.InputText} .\n";
                    output += $"your response : {session.OutputText} .\n"; // Correction ici
                    output += $"diff: {session.Diff} .\n";
                    _logger.LogTrace($"Ligne d'historique ajoutée: Utilisateur - {session.InputText}, Assistant - {session.OutputText}, Diff - {session.Diff}");
                }
                _logger.LogDebug($"Historique de la session utilisateur récupéré: {output}");
                return output;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de l'historique de la session utilisateur.");
                return string.Empty;
            }   

        }


        public  async Task<string> ProcessTextAsync(string inputText)
        {
            _logger.LogInformation($"Traitement du texte entrant: {inputText}");
            try
            {

                //InitializeUsingModelName();

                var textResult = string.Empty;
                var resultList = new List<string[]>();

                if (!_subdiviseText && inputText.Contains("QFIA"))
                    return await AskQuestionForIaAsync(inputText);
                else if (!_subdiviseText)
                    return await ProcessTextPartAsync(inputText);


                _logger.LogInformation($"Division du text: {inputText}");
                var textParts = TextHelper.DivideTextIntoParts(inputText, _maxCharsPerPartGemmini);

                if (inputText.Contains("QFIA"))
                {

                    var isNotFirst = false;
                    foreach (var input in textParts)
                    {
                        var prompt = input;
                        if (isNotFirst)
                        {
                            prompt = $". Voici la suite de tu text : {input}";
                        }
                        var output = await AskQuestionForIaAsync(prompt);

                        resultList.Add(new string[] { input, output }); //(new st inpoutText, outputText);
                        isNotFirst = true;
                    }


                }
                else
                {
                    foreach (var input in textParts)
                    {
                        var output = await ProcessTextPartAsync(input);
                        resultList.Add(new string[] { input, output }); //(new st inpoutText, outputText);
                    }
                }

                foreach (var result in resultList)
                {
                    textResult += result[1];
                }


                if (textResult != null)
                {
                    textResult = textResult.Trim();
                    
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


        public async Task<string> AskQuestionForIaAsync(string questionText)
        {
            _logger.LogInformation($"Question pour l'IA: {questionText}");
            try
            {
                questionText = questionText.Replace("QFIA", "");
                _history.AddUserMessage(questionText);

                var result = await _chatService.GetChatMessageContentAsync(_history);
                var textResult = result.Content;
                _logger.LogInformation($"Réponse de l'IA: {textResult}");
                Debug.WriteLine(textResult);
                Console.WriteLine(textResult); // Utile pour le débogage initial dans Docker
                if (result != null)
                {
                    _history.Add(result);
                }

                if (textResult != null)
                {

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
                _logger.LogError(ex, $"Erreur lors du traitement du texte: {questionText}");
                return $"Erreur lors du traitement : {questionText}";
            }
        }


        public async Task<string> ProcessTextPartAsync(string inputText)
        {
            _logger.LogInformation($"Traitement du texte entrant: {inputText}");
            try
            {
               
                var messageType = MessageType.Corrector;
                if (_className.Contains("Translator"))
                    messageType = MessageType.Translator;
                //TODO lectur dans la base
                var inBase = await _sessionRepository.GetSessionAsync(inputText, messageType,ModelName);
                if(inBase!=null)
                    return inBase.OutputText;

                var startDate = DateTime.UtcNow;
                var userMessage = $"{BottomOfThequestion} (retourne seulement le texte) : {inputText}";
                _history.AddUserMessage(userMessage);
                _logger.LogDebug($"Message utilisateur (correction) ajouté à l'historique: {userMessage}");


                _logger.LogInformation("Appel du service de complétion de chat.");
                var result = await _chatService.GetChatMessageContentAsync(_history);
                var textResult = result.Content;
                _logger.LogInformation($"Réponse du service de complétion: {textResult}");
               // Debug.WriteLine(textResult);
               // Console.WriteLine(textResult); // Utile pour le débogage initial dans Docker
                if (result != null)
                {
                    _history.Add(result);
                }

                if (textResult != null)
                {
                    textResult= textResult.Replace("Voici le texte corrigé :\n\n", string.Empty).Trim();
                    var diff = TextHelper.GenerateCharacterDiff(inputText, textResult);
                    _history.AddUserMessage($"differences : {diff}");
                    _logger.LogDebug($"Différences générées: {diff}");
                    _retyCount = 1;


                    if(textResult.ToUpper().Contains("INTRADUCTIBLE"))
                    {
                        textResult = inputText;
                        textResult = inputText;
                    }
                    //save in database

                    // Sauvegarde de la session
                  
                    var processingTime = DateTime.UtcNow - startDate;
                    await _sessionRepository.AddAsync(new Session(new Guid(),inputText, textResult, diff,DateTime.UtcNow, processingTime,CorrectionStatus.Completed, messageType, ModelName));

                    return textResult;
                }
                else
                {
                    _logger.LogWarning("La réponse du service de complétion est nulle.");
                    return "Erreur lors du traitement : Réponse du service nulle.";
                }
            }
            //catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            //{
            //    _logger.LogError(ex, "Erreur 429 : Trop de requêtes envoyées à l'API.");
            //    await Task.Delay(500);
            //    return await ProcessTextPartAsync(inputText);

            //}
            //catch (HttpRequestException ex)
            //{
            //    _logger.LogError(ex, $"HttpRequestException sans code 429. StatusCode: {ex.StatusCode}");
            //    return $"Erreur HTTP lors du traitement : {inputText}";
            //}
            catch (Exception ex)
            {

                Debug.WriteLine($"$\"Erreur lors du traitement du texte: {inputText} : {ex}");
                _logger.LogError(ex, $"Erreur lors du traitement du texte: {inputText}");
                var delayTime = 1000 * _retyCount*_minDelayTime;
                Debug.WriteLine($"delayTime : {delayTime} ");
                await Task.Delay(delayTime);
                _retyCount++;
                return await ProcessTextPartAsync(inputText);
               // return $"Erreur lors du traitement : {inputText}";
            }
        }






    }


}

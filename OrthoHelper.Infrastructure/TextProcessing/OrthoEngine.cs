using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using OllamaSharp;
using Tools;

namespace OrthoHelper.Infrastructure.TextProcessing
{


    
    public class OrthoEngine : IOrthoEngine
    {
        private readonly IChatCompletionService _chatService;
        private readonly ChatHistory _history;
        //  private readonly ILogger<OrthoService> _logger;
        public OrthoEngine(HttpClient httpClient)
        {

            //_logger = logger;
            //_modelName = configuration["OllamaSettings:ModelName"];
            //_ollamaAdress = configuration["OllamaSettings:Address"];


            try
            {
                //var httpClient = new HttpClient
                //{
                //    Timeout = TimeSpan.FromMinutes(15),
                //};
#pragma warning disable SKEXP0001
                // _chatService = new OllamaApiClient(httpClient).AsChatCompletionService();
                _chatService = new OllamaApiClient(httpClient, "gemma3").AsChatCompletionService();

#pragma warning restore SKEXP0001
                _history = new();
                //_history.AddSystemMessage("Tu es une IA spécialiste en correction d'orthographe. À chaque texte que tu reçois, retourne le texte corrigé sans rien d'autre.");
                var initText = "";
                //initText = "Tu es une IA spécialiste en correction d'orthographe, et de grammaire.";

                initText = "Tu es un assistant pour mon éditeur de texte Obsidian. Tu me parles exclusivement en français même si je te parle dans une autre langue, tu me réponds en français. \r\nTu dois pouvoir me corriger mes fautes d'orthographe ou reformuler mes phrases si elles sont grammaticalement incorrectes. Si le text est du markdown, tu ne dois pas le modifier.";


                _history.AddSystemMessage(initText);
            }
            catch (Exception ex)
            {
              //  _logger.LogError(ex, "Erreur lors de l'initialisation du service Ollama");
                throw;
            }
        }
        // Avant (synchrone)
        // public string ProcessText(string inputText) { ... }

        // Après (asynchrone)
        public async Task<string> ProcessTextAsync(string inputText)
        {
            // Exemple de logique synchrone wrappée dans une Task
            try
            {

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
                return (textResult);
            }
            catch (Exception ex)
            {
               // _logger.LogError(ex, "Erreur lors du traitement du texte");
               // return ("Erreur lors du traitement : " + text, "");
                return ("Erreur lors du traitement : " + inputText);
            }
        }
    }
}

using System.Diagnostics;
using Microsoft.Extensions.Logging; // Ajoutez cet using
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;
using OllamaSharp;
using OrthoHelper.Domain.Features.Common.Ports;
using OrthoHelper.Domain.Features.TextCorrection.Ports;
using OrthoHelper.Domain.Features.TextCorrection.Ports.Repositories;
using Tools;

namespace OrthoHelper.Infrastructure.Features.TextProcessing
{
    public class OrthoEngineCorrector : OrthoEngine
    {
        public override string InitText =>  "Tu es un assistant pour mon éditeur de texte Obsidian. Tu me parles exclusivement en français même si je te parle dans une autre langue, tu me réponds en français. \r\nTu dois pouvoir me corriger mes fautes d'orthographe ou reformuler mes phrases si elles sont grammaticalement incorrectes. Si le text est du markdown, tu ne dois pas le modifier.";
        public override string BottomOfThequestion => "Peux-tu corriger ?";




        // Constructeur mis à jour pour accepter ILogger
        public OrthoEngineCorrector(HttpClient httpClient, ISessionRepository repository, ICurrentUserService currentUserService, ILogger<OrthoEngineCorrector> logger)
            :base(httpClient, repository, currentUserService, logger)
        {
           
        }



     
        public async Task InitializeUserSession(string username)
        {
            _logger.LogInformation($"Initialisation de la session utilisateur pour: {username}");
            var sessions = await _sessionRepository.GetSessionsAsync();
            _logger.LogDebug($"Nombre de sessions de correction récupérées: {sessions.Count()}");

            foreach (var session in sessions)
            {
                _history.AddUserMessage(session.InputText);
                _history.AddAssistantMessage(session.OutputText);
                _logger.LogTrace($"Message utilisateur ajouté à l'historique: {session.InputText}");
                _logger.LogTrace($"Réponse de l'assistant ajoutée à l'historique: {session.OutputText}");
            }
        }

     
      



          
    }
}
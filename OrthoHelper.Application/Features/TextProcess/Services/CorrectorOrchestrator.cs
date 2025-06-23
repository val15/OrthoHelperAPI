using OrthoHelper.Domain.Features.TextCorrection.Entities;
using OrthoHelper.Domain.Features.TextCorrection.Ports.Repositories;
using OrthoHelper.Domain.Features.TextCorrection.Ports;
using OrthoHelper.Domain.Features.TextCorrection.ValueObjects;
using OllamaSharp.Models.Chat;
using OrthoHelper.Infrastructure.Features.TextProcessing.Entities;
using static OrthoHelper.Domain.Features.TextCorrection.Entities.Session;
using OrthoHelper.Domain.Features.TextProcess;

namespace OrthoHelper.Application.Features.TextCorrection.Services
{

    public class CorrectorOrchestrator : ICorrectorOrchestrator
    {
        private readonly ITextProcessingEngine _textProcessingEngine;
        private readonly ILLMModelRepository _llmModelRepository;

        public CorrectorOrchestrator(
            ITextProcessingEngine textProcessingEngine,
            ILLMModelRepository llmModelRepository)
        {
            _textProcessingEngine = textProcessingEngine;
            _llmModelRepository = llmModelRepository;
        }

        public async Task<Session> ProcessAsync(TextToCorrect text, ModelName modelName, string username)
        {
            // Récupération des modèles disponibles
            var availableModels = await _llmModelRepository.GetAvailableLLMModelsAsync();

            // Création de l'entité Domain
            var correctionSession = Session.Create(text.Value);

            // Configuration du modèle
            correctionSession.SetModelName(modelName.Value,
                _textProcessingEngine,
                availableModels,EngineType.Corrector);

            // Appel au moteur de traitement
            var correctedText = await _textProcessingEngine.ProcessTextAsync(text.Value,EngineType.Corrector);

            // Mise à jour de l'entité Domain
            correctionSession.ApplyCorrection(correctedText);
            correctionSession.Type = MessageType.Corrector;
            return correctionSession;
        }
    }
}

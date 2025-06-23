using OrthoHelper.Domain.Features.TextCorrection.Entities;
using OrthoHelper.Domain.Features.TextCorrection.Ports;
using OrthoHelper.Domain.Features.TextCorrection.Ports.Repositories;
using OrthoHelper.Domain.Features.TextCorrection.ValueObjects;
using OrthoHelper.Domain.Features.TextProcess;
using OrthoHelper.Infrastructure.Features.TextProcessing.Entities;
using static OrthoHelper.Domain.Features.TextCorrection.Entities.Session;

namespace OrthoHelper.Application.Features.TextCorrection.Services
{

    public class TranslatorOrchestrator : ITranslatorOrchestrator
    {
        private readonly ITextProcessingEngine _textProcessingEngine;
        private readonly ILLMModelRepository _llmModelRepository;

        public TranslatorOrchestrator(
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
            var session = Session.Create(text.Value);

            // Configuration du modèle
            session.SetModelName(modelName.Value, _textProcessingEngine, availableModels, EngineType.Translator);

            // Appel au moteur de traitement
            var responceText = await _textProcessingEngine.ProcessTextAsync(text.Value, EngineType.Translator);

            // Mise à jour de l'entité Domain
            session.ApplyCorrection(responceText);
            session.Type = MessageType.Translator;
            return session;
        }
    }
}

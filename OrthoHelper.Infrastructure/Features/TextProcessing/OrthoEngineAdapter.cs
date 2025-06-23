using OrthoHelper.Domain.Features.TextCorrection.Ports;
using OrthoHelper.Domain.Features.TextProcess;
using OrthoHelper.Domain.Features.TextProcess.Ports;

namespace OrthoHelper.Infrastructure.Features.TextProcessing
{
    public class OrthoEngineAdapter : ITextProcessingEngine
    {
        private readonly ITranslatorEngine _translatorEngine;
        private readonly ICorrectorEngine _correctorEngine;

        public OrthoEngineAdapter(ITranslatorEngine translatorEngine, ICorrectorEngine correctorEngine)
        {
            _translatorEngine = translatorEngine;
            _correctorEngine = correctorEngine;
        }

        public void SetModelName(string modelName, EngineType engineType)
        {
            if (engineType == EngineType.Translator)
                _translatorEngine.SetModelName(modelName);
            else
                _correctorEngine.SetModelName(modelName);
        }

        public async Task<string> ProcessTextAsync(string inputText, EngineType engineType)
        {
            if (engineType == EngineType.Translator)
                return await _translatorEngine.ProcessTextAsync(inputText);
            else
                return await _correctorEngine.ProcessTextAsync(inputText);
        }

        public string GetModelName(EngineType engineType)
        {
            return engineType == EngineType.Translator
                ? _translatorEngine.GetModelName()
                : _correctorEngine.GetModelName();
        }
    }
}

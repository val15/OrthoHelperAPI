using OrthoHelper.Domain.Ports;

namespace OrthoHelper.Infrastructure.TextProcessing
{
    public class OrthoEngineAdapter : ITextProcessingEngine
    {
        private readonly IOrthoEngine _orthoEngine;

        public OrthoEngineAdapter(IOrthoEngine orthoEngine)
        {
            _orthoEngine = orthoEngine;
        }

        public async  Task<string> CorrectTextAsync(string inputText)
        {
            return await _orthoEngine.ProcessTextAsync(inputText);
        }
    }
}

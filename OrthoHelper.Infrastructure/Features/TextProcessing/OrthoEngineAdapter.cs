using OrthoHelper.Domain.Features.TextCorrection.Ports;

namespace OrthoHelper.Infrastructure.Features.TextProcessing
{
    public class OrthoEngineAdapter : ITextProcessingEngine
    {
        private readonly IOrthoEngine _orthoEngine;

        public OrthoEngineAdapter(IOrthoEngine orthoEngine)
        {
            _orthoEngine = orthoEngine;
        }

        public async Task<string> CorrectTextAsync(string inputText)
        {
            return await _orthoEngine.ProcessTextAsync(inputText);
        }

        //public async Task InitializeUserSession(string username)
        //{
        //    await _orthoEngine.InitializeUserSession(username);
        //}
    }
}

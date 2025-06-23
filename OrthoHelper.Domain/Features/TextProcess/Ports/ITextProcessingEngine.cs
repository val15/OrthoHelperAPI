using OrthoHelper.Domain.Features.TextProcess;

namespace OrthoHelper.Domain.Features.TextCorrection.Ports
{
    // OrthoHelper.Domain/Ports/ITextProcessingEngine.cs
    public interface ITextProcessingEngine
    {
        Task<string> ProcessTextAsync(string inputText, EngineType engineType);
        void SetModelName(string modelName, EngineType engineType);
        string GetModelName(EngineType engineType);

        //Task InitializeUserSession(string username);
    }
}

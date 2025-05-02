namespace OrthoHelper.Infrastructure.Features.TextProcessing
{
    // IOrthoEngine.cs
    public interface IOrthoEngine
    {
        Task<string> ProcessTextAsync(string inputText);

        void SetModelName(string modelName);
        //Task InitializeUserSession(string username);
    }
}

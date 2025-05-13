namespace OrthoHelper.Domain.Features.TextCorrection.Ports
{
    // IOrthoEngine.cs
    public interface IOrthoEngine
    {
        Task<string> ProcessTextAsync(string inputText);

        void SetModelName(string modelName);

        string GetModelName();
        //Task InitializeUserSession(string username);
    }
}

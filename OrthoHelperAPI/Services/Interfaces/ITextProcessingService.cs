using OrthoHelperAPI.Model;

namespace OrthoHelperAPI.Services.Interfaces
{
    public interface ITextProcessingService
    {
        Task<(string correctedText, string diffText)> ProcessTextAsync(string text, List<Message> messagesList = null);


    }
}

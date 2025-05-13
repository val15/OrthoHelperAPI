namespace OrthoHelper.Domain.Features.TextTranslation.Ports
{
    public interface IHtmlParser
    {
        Task<string> ExtractTranslatableTextAsync(string htmlFilePath);
        Task<string> ReplaceTranslatedTextAsync(string htmlFilePath, Dictionary<string, string> translations, string modelName, string  translatedHtmlPath=null);
    }
}


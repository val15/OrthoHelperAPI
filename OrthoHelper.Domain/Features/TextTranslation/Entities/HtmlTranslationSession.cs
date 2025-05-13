using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrthoHelper.Domain.Features.TextTranslation.Entities
{
    public class HtmlTranslationSession
    {
        public Guid Id { get; private set; }
        public string OriginalHtmlPath { get; private set; }
        public string TranslatedHtmlPath { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private HtmlTranslationSession(string originalHtmlPath)
        {
            Id = Guid.NewGuid();
            OriginalHtmlPath = originalHtmlPath;
            CreatedAt = DateTime.UtcNow;
        }

        public static HtmlTranslationSession Create(string originalHtmlPath)
        {
            if (string.IsNullOrWhiteSpace(originalHtmlPath))
                throw new InvalidHtmlFileException("Le chemin du fichier HTML ne peut pas être vide.");

            return new HtmlTranslationSession(originalHtmlPath);
        }

        public void SetTranslatedHtmlPath(string translatedHtmlPath)
        {
            TranslatedHtmlPath = translatedHtmlPath;
        }
    }

    
    public class InvalidHtmlFileException : Exception
    {
        public InvalidHtmlFileException()
        {
        }

        public InvalidHtmlFileException(string? message) : base(message)
        {
        }

        public InvalidHtmlFileException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}


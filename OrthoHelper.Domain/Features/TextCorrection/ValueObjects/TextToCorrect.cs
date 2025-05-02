using OrthoHelper.Domain.Features.TextCorrection.Exceptions;

namespace OrthoHelper.Domain.Features.TextCorrection.ValueObjects
{
    public class TextToCorrect
    {
        public string Value { get; }

        public TextToCorrect(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidTextException("Le texte à corriger ne peut pas être vide.");
            Value = value;
        }
    }
}

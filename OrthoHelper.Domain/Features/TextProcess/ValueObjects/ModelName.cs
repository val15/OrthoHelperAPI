using OrthoHelper.Domain.Features.TextCorrection.Exceptions;

namespace OrthoHelper.Domain.Features.TextCorrection.ValueObjects
{
    public class ModelName
    {
        public string Value { get; }

        public ModelName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidModelNameException("Le nom du modèle ne peut pas être vide.");
            Value = value;
        }
    }
}

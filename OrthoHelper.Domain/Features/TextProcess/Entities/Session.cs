using OrthoHelper.Domain.Features.TextCorrection.Exceptions;
using OrthoHelper.Domain.Features.TextCorrection.Ports;
using OrthoHelper.Domain.Features.TextProcess;
using Tools;

namespace OrthoHelper.Domain.Features.TextCorrection.Entities
{
    // OrthoHelper.Domain/Entities/CorrectionSession.cs
    public class Session
    {
        public Guid Id { get;  set; }
        public string InputText { get;  set; }
        public string OutputText { get;  set; }
        public DateTime CreatedAt { get;  set; }
        public CorrectionStatus Status { get; private set; }
        public string Diff { get; set; } = string.Empty;
        public TimeSpan ProcessingTime { get; private set; }
        public MessageType Type { get; set; }  // Correction ou Traduction
        public string ModelName { get; set; } = "Ollama:Gemma3"; // Nom du modèle utilisé pour la correction ou la traduction



        // Constructor privé pour contrôler la création
        private Session(string originalText)
        {

            Id = Guid.NewGuid();
            InputText = originalText;
            CreatedAt = DateTime.UtcNow;
            Status = CorrectionStatus.Pending;
        }
        public enum MessageType
        {
            Corrector,
            Translator
        }

        public Session(Guid id, string originalText, string correctedText,string diff, DateTime createdAt, TimeSpan processingTime, CorrectionStatus status,
            MessageType type,string modelName)
        {
            Id = id;
            InputText = originalText;
            OutputText = correctedText;
            Diff = diff;
            CreatedAt = createdAt;
            ProcessingTime = processingTime;
            Status = status;
            ModelName = modelName;
            Type = type;

        }

        // Factory method pour appliquer des règles métier
        public static Session Create(string originalText)
        {
            if (string.IsNullOrWhiteSpace(originalText))
                throw new InvalidTextException("Le texte ne peut pas être vide.");

            return new Session(originalText);
        }
        // Méthode pour définir le modèle
        public void SetModelName(string modelName, 
            ITextProcessingEngine textProcessingEngine,
            IEnumerable<LLMModel> availableModels,
             EngineType engineType)
        {
            if (string.IsNullOrWhiteSpace(modelName))
                throw new InvalidModelNameException("Le nom du modèle ne peut pas être vide.");
            if (availableModels.FirstOrDefault(x=> modelName.Contains(x.ModelName)) == null )
                throw new InvalidModelNameException("Le modèle spécifié n'est pas disponible.");
            textProcessingEngine.SetModelName(modelName, engineType);
            // Stocker le modèle dans l'entité si nécessaire
            ModelName = modelName;
        }

        // Méthode pour appliquer la correction
        public void ApplyCorrection(string correctedText)
        {
            if (Status != CorrectionStatus.Pending)
                throw new InvalidOperationException("La correction a déjà été appliquée.");

            OutputText = correctedText;
            Diff =  TextHelper.GenerateCharacterDiff(InputText, OutputText);
            ProcessingTime = DateTime.UtcNow - CreatedAt;
            Status = CorrectionStatus.Completed;
        }
    }

    // Enum pour le statut
    public enum CorrectionStatus
    {
        Pending,
        Completed,
        Failed
    }
}

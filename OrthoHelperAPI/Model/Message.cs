using OrthoHelper.Infrastructure.Features.TextProcessing.Entities;

namespace OrthoHelperAPI.Model
{
    public class Message
    {
        public int Id { get; set; }
        public string InputText { get; set; }
        public string OutputText { get; set; }
        public string Diff { get; set; }
        public TimeSpan ProcessingTime { get; set; }

        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public string ModelName { get; set; } = "DefaultModel"; // Nom du modèle utilisé pour la correction ou la traduction
        public MessageType? Type { get; set; } = MessageType.Corrector; // Correction ou Traduction

    }
    public enum MessageType
    {
        Corrector,
        Translator
    }
}

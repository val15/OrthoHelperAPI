using OrthoHelper.Domain.Features.Auth.Entities;
using static OrthoHelper.Domain.Features.TextCorrection.Entities.Session;

namespace OrthoHelper.Infrastructure.Features.TextProcessing.Entities
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
        public string ModelName { get; set; }
        public MessageType? Type { get; set; }  // Correction ou Traduction
    }

   
}

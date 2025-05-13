using OrthoHelper.Domain.Features.TextCorrection.Entities;
using OrthoHelper.Infrastructure.Features.TextProcessing.Entities;

namespace OrthoHelper.Infrastructure.Features.TextProcessing.Mappings
{
    // Infrastructure/Features/CorrectionSession/Mappings/CorrectionSessionAdapter.cs
    public static class CorrectionSessionAdapter
    {
        // Conversion Message (Infra) -> CorrectionSession (Domaine)
        public static Session ToDomain(Message message)
        {
            return new Session(
                id: message.Id.ToGuid(), // Adaptez selon le type réel
                originalText: message.InputText,
                correctedText: message.OutputText,
                processingTime: message.ProcessingTime,
                diff: message.Diff,
                createdAt: message.CreatedAt,
                status: ParseStatus(message) // Méthode helper
            );
        }

        public static Guid ToGuid(this int id)
        {
            byte[] bytes = new byte[16];
            BitConverter.GetBytes(id).CopyTo(bytes, 0);
            return new Guid(bytes);
        }

        // Conversion CorrectionSession (Domaine) -> Message (Infra)
        public static Message FromDomain(Session session, int userId)
        {
            return new Message
            {
                InputText = session.InputText,
                OutputText = session.OutputText,
                Diff = session.Diff,
                CreatedAt = session.CreatedAt,
                UserId = userId,
                ProcessingTime = session.ProcessingTime// À implémenter
            };
        }

        private static CorrectionStatus ParseStatus(Message message)
        {
            // Logique de conversion si nécessaire
            return CorrectionStatus.Completed;
        }
    }
}

using OrthoHelper.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrthoHelper.Domain.Entities
{
    // OrthoHelper.Domain/Entities/CorrectionSession.cs
    public class CorrectionSession
    {
        public Guid Id { get; private set; }
        public string OriginalText { get; private set; }
        public string CorrectedText { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public CorrectionStatus Status { get; private set; }

        // Constructor privé pour contrôler la création
        private CorrectionSession(string originalText)
        {
            Id = Guid.NewGuid();
            OriginalText = originalText;
            CreatedAt = DateTime.UtcNow;
            Status = CorrectionStatus.Pending;
        }

        // Factory method pour appliquer des règles métier
        public static CorrectionSession Create(string originalText)
        {
            if (string.IsNullOrWhiteSpace(originalText))
                throw new InvalidTextException("Le texte ne peut pas être vide.");

            return new CorrectionSession(originalText);
        }

        // Méthode pour appliquer la correction
        public void ApplyCorrection(string correctedText)
        {
            if (Status != CorrectionStatus.Pending)
                throw new InvalidOperationException("La correction a déjà été appliquée.");

            CorrectedText = correctedText;
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

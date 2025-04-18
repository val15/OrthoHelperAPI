﻿using OrthoHelper.Domain.Features.TextCorrection.Exceptions;
using Tools;

namespace OrthoHelper.Domain.Features.TextCorrection.Entities
{
    // OrthoHelper.Domain/Entities/CorrectionSession.cs
    public class CorrectionSession
    {

        public Guid Id { get;  set; }
        public string OriginalText { get;  set; }
        public string CorrectedText { get;  set; }
        public DateTime CreatedAt { get;  set; }
        public CorrectionStatus Status { get; private set; }
        public string Diff { get; set; } = string.Empty;
        public TimeSpan ProcessingTime { get; private set; }

       
        // Constructor privé pour contrôler la création
        private CorrectionSession(string originalText)
        {

            Id = Guid.NewGuid();
            OriginalText = originalText;
            CreatedAt = DateTime.UtcNow;
            Status = CorrectionStatus.Pending;
        }

        public CorrectionSession(Guid id, string originalText, string correctedText,string diff, DateTime createdAt, TimeSpan processingTime, CorrectionStatus status)
        {
            Id = id;
            OriginalText = originalText;
            CorrectedText = correctedText;
            Diff = diff;
            CreatedAt = createdAt;
            ProcessingTime = processingTime;
            Status = status;
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
            Diff =  TextHelper.GenerateCharacterDiff(OriginalText, CorrectedText);
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

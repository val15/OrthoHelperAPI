using OrthoHelper.Domain.Entities;
using OrthoHelper.Domain.Ports;

namespace OrthoHelper.Domain.Services
{
    public class CorrectionOrchestrator
    {
        private readonly ITextProcessingEngine _textEngine;

        public CorrectionOrchestrator(ITextProcessingEngine textEngine)
        {
            _textEngine = textEngine;
        }

        public async Task<CorrectionSession> ProcessCorrectionAsync(string text)
        {
            var session = CorrectionSession.Create(text);
            var correctedText = await _textEngine.CorrectTextAsync(text); // Langue auto-détectée
            session.ApplyCorrection(correctedText);
            return session;
        }
    }
}

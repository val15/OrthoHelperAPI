using OrthoHelper.Domain.Features.TextCorrection.Entities;
using OrthoHelper.Domain.Features.TextCorrection.Ports;

namespace OrthoHelper.Domain.Features.TextCorrection.Services
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

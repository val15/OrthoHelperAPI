using OrthoHelper.Domain.Features.TextCorrection.Entities;
using OrthoHelper.Domain.Features.TextCorrection.ValueObjects;

namespace OrthoHelper.Domain.Features.TextCorrection.Ports
{
    public interface ICorrectorOrchestrator
    {
        Task<Session> ProcessAsync(TextToCorrect text, ModelName modelName, string username);
    }
}

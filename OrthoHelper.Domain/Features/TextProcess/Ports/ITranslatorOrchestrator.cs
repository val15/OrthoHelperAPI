using OrthoHelper.Domain.Features.TextCorrection.Entities;
using OrthoHelper.Domain.Features.TextCorrection.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrthoHelper.Domain.Features.TextCorrection.Ports
{
    public interface ITranslatorOrchestrator
    {
        Task<Session> ProcessAsync(TextToCorrect text, ModelName modelName, string username);
    }
}

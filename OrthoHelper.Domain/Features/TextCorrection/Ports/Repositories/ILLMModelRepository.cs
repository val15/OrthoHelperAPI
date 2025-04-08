using OrthoHelper.Domain.Features.TextCorrection.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrthoHelper.Domain.Features.TextCorrection.Ports.Repositories
{
    public interface  ILLMModelRepository
    {

        Task<IEnumerable<LLMModel>> GetAvailableLLMModelsAsync();
    }
}

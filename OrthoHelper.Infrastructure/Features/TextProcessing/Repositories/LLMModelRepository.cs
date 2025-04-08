using OrthoHelper.Domain.Features.TextCorrection.Entities;
using OrthoHelper.Domain.Features.TextCorrection.Ports.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrthoHelper.Infrastructure.Features.TextProcessing.Repositories
{
    public class LLMModelRepository : ILLMModelRepository
    {
        public Task<IEnumerable<LLMModel>> GetAvailableLLMModelsAsync()
        {
            
            IEnumerable<LLMModel> llmModelList = new List<LLMModel>
            {
                new LLMModel("Ollama:Gemma")
                ,
                new LLMModel("Online:gemini-2.0-flash")
                
            };
            return Task.FromResult(llmModelList);
        }
    }
}

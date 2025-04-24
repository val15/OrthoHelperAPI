using OrthoHelper.Domain.Features.TextCorrection.Entities;

namespace OrthoHelper.Domain.Features.TextCorrection.Ports.Repositories
{
    public interface  ILLMModelRepository
    {

        Task<IEnumerable<LLMModel>> GetAvailableLLMModelsAsync();
    }
}

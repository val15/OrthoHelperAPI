using AutoMapper;
using MediatR;
using OrthoHelper.Application.Features.TextCorrection.DTOs;
using OrthoHelper.Domain.Features.TextCorrection.Ports.Repositories;

namespace OrthoHelper.Application.Features.TextCorrection.Queries
{
    //internal class BrowseCorrectionSessionQuery
    //{
    //}

   
    public class BrowseLLMModelsQuery : IRequest<IEnumerable<LLMModelOutputDto>>
    {
        //public string UserName { get; set; }
    }

    // Handler
    public class BrowseLLMModelsQueryHandler : IRequestHandler<BrowseLLMModelsQuery, IEnumerable<LLMModelOutputDto>>
    {
        private readonly ILLMModelRepository _llmModelRepository;
        private readonly IMapper _mapper;


        public BrowseLLMModelsQueryHandler(ILLMModelRepository llmModelRepository, IMapper mapper)
        {
            _llmModelRepository = llmModelRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<LLMModelOutputDto>> Handle(BrowseLLMModelsQuery request, CancellationToken cancellationToken)
        {
            var availableModels = await _llmModelRepository.GetAvailableLLMModelsAsync();

            return _mapper.Map<IEnumerable<LLMModelOutputDto>>(availableModels);
        }
    }

}

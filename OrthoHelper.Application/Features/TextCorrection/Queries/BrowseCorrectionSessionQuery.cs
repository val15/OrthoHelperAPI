using AutoMapper;
using MediatR;
using OrthoHelper.Application.Features.TextCorrection.DTOs;
using OrthoHelper.Domain.Features.TextCorrection.Ports.Repositories;

namespace OrthoHelper.Application.Features.TextCorrection.Queries
{
    //internal class BrowseCorrectionSessionQuery
    //{
    //}

    public class BrowseCorrectionSessionQuery : IRequest<IEnumerable<CorrectTextOutputDto>>
    {
        //public string UserName { get; set; }
    }

    // Handler
    public class BrowseCorrectionSessionQueryHandler : IRequestHandler<BrowseCorrectionSessionQuery, IEnumerable<CorrectTextOutputDto>>
    {
        private readonly ICorrectionSessionRepository _correctionSessionRepository;
        private readonly IMapper _mapper;


        public BrowseCorrectionSessionQueryHandler(ICorrectionSessionRepository correctionSessionRepository, IMapper mapper)
        {
            _correctionSessionRepository = correctionSessionRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CorrectTextOutputDto>> Handle(BrowseCorrectionSessionQuery request, CancellationToken cancellationToken)
        {
            var correctionSessions = await _correctionSessionRepository.GetCorrectionSessionsAsync();
            return _mapper.Map<IEnumerable<CorrectTextOutputDto>>(correctionSessions);
        }
    }

}

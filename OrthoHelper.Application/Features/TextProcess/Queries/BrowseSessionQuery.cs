using AutoMapper;
using MediatR;
using OrthoHelper.Application.Features.TextCorrection.DTOs;
using OrthoHelper.Domain.Features.TextCorrection.Ports.Repositories;

namespace OrthoHelper.Application.Features.TextCorrection.Queries
{

    public class BrowseSessionQuery : IRequest<IEnumerable<OutputTextDto>>
    {
        //public string UserName { get; set; }
    }

    // Handler
    public class BrowseCorrectionSessionQueryHandler : IRequestHandler<BrowseSessionQuery, IEnumerable<OutputTextDto>>
    {
        private readonly ISessionRepository _correctionSessionRepository;
        private readonly IMapper _mapper;


        public BrowseCorrectionSessionQueryHandler(ISessionRepository correctionSessionRepository, IMapper mapper)
        {
            _correctionSessionRepository = correctionSessionRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OutputTextDto>> Handle(BrowseSessionQuery request, CancellationToken cancellationToken)
        {
            var correctionSessions = await _correctionSessionRepository.GetSessionsAsync();
            return _mapper.Map<IEnumerable<OutputTextDto>>(correctionSessions);
        }
    }

}

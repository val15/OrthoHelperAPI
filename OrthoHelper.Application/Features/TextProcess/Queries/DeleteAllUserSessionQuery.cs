using AutoMapper;
using MediatR;
using OrthoHelper.Application.Features.TextCorrection.DTOs;
using OrthoHelper.Domain.Features.TextCorrection.Ports.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrthoHelper.Application.Features.TextCorrection.Queries
{


    public class DeleteAllUserSessionQuery : IRequest<int>
    {
        //public string UserName { get; set; }
    }

    // Handler
    public class DeleteAllUserCorrectionSessionQueryHandler : IRequestHandler<DeleteAllUserSessionQuery,int>
    {
        private readonly ISessionRepository _correctionSessionRepository;
        private readonly IMapper _mapper;


        public DeleteAllUserCorrectionSessionQueryHandler(ISessionRepository correctionSessionRepository)
        {
            _correctionSessionRepository = correctionSessionRepository;
        }

        public async Task<int> Handle(DeleteAllUserSessionQuery request, CancellationToken cancellationToken)
        {
           return await _correctionSessionRepository.DeleteAllUserCorrectionSessionsAsync();
        }
    }
}

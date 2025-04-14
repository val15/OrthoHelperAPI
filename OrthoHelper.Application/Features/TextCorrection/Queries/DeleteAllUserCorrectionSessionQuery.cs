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


    public class DeleteAllUserCorrectionSessionQuery : IRequest<int>
    {
        //public string UserName { get; set; }
    }

    // Handler
    public class DeleteAllUserCorrectionSessionQueryHandler : IRequestHandler<DeleteAllUserCorrectionSessionQuery,int>
    {
        private readonly ICorrectionSessionRepository _correctionSessionRepository;
        private readonly IMapper _mapper;


        public DeleteAllUserCorrectionSessionQueryHandler(ICorrectionSessionRepository correctionSessionRepository)
        {
            _correctionSessionRepository = correctionSessionRepository;
        }

        public async Task<int> Handle(DeleteAllUserCorrectionSessionQuery request, CancellationToken cancellationToken)
        {
           return await _correctionSessionRepository.DeleteAllUserCorrectionSessionsAsync();
        }
    }
}

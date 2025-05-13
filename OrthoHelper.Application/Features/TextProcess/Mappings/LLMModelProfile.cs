using AutoMapper;
using OrthoHelper.Application.Features.TextCorrection.DTOs;
using OrthoHelper.Domain.Features.TextCorrection.Entities;

namespace OrthoHelper.Application.Features.TextCorrection.Mappings
{
    public class LLMModelProfile : Profile
    {
        public LLMModelProfile()
        {
            CreateMap<LLMModel, LLMModelOutputDto>();

        }
    }
}

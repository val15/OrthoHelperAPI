using AutoMapper;
using OrthoHelper.Application.Features.TextCorrection.DTOs;
using OrthoHelper.Domain.Features.TextCorrection.Entities;

namespace OrthoHelper.Domain.Features.TextCorrection.Mappings
{
    public class CorrectionSessionProfile : Profile
    {
        public CorrectionSessionProfile()
        {
            CreateMap<CorrectionSession, CorrectTextOutputDto>()
                // Mapping des propriétés avec noms différents
                .ForMember(dest => dest.InputText, opt => opt.MapFrom(src => src.OriginalText))
                .ForMember(dest => dest.OutputText, opt => opt.MapFrom(src => src.CorrectedText))

                // Mapping des propriétés directes
                .ForMember(dest => dest.Diff, opt => opt.MapFrom(src => src.Diff))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

            // Gestion des propriétés manquantes

           // CreateMap<CorrectionSession, CorrectTextOutputDto>();
            CreateMap<CorrectTextInputDto, CorrectionSession>();

        }
    }

   
}

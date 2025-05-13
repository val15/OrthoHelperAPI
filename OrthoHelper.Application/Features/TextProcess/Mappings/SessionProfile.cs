using AutoMapper;
using OrthoHelper.Application.Features.TextCorrection.DTOs;
using OrthoHelper.Domain.Features.TextCorrection.Entities;

namespace OrthoHelper.Domain.Features.TextCorrection.Mappings
{
    public class SessionProfile : Profile
    {
        public SessionProfile()
        {
            CreateMap<Session, OutputTextDto>()
                // Mapping des propriétés avec noms différents
                .ForMember(dest => dest.InputText, opt => opt.MapFrom(src => src.InputText))
                .ForMember(dest => dest.OutputText, opt => opt.MapFrom(src => src.OutputText))

                // Mapping des propriétés directes
                .ForMember(dest => dest.Diff, opt => opt.MapFrom(src => src.Diff))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

            // Gestion des propriétés manquantes

           // CreateMap<CorrectionSession, CorrectTextOutputDto>();
            CreateMap<InputTextDto, Session>();

        }
    }

   
}

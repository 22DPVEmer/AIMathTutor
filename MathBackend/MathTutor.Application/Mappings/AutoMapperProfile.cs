using AutoMapper;
using MathTutor.Application.DTOs;
using MathTutor.Core.Entities;
using MathTutor.Core.Models;

namespace MathTutor.Application.Mappings;

public class ApplicationAutoMapperProfile : Profile
{
    public ApplicationAutoMapperProfile()
    {
        // DTO mappings
        CreateMap<CreateMathProblemDto, MathProblem>();
        CreateMap<UpdateMathProblemDto, MathProblem>();
        CreateMap<MathProblem, MathProblemDto>()
            .ForMember(dest => dest.TopicName, opt => opt.MapFrom(src => src.Topic.Name));
    }
}

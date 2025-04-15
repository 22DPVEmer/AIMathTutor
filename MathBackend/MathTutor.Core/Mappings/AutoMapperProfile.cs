using AutoMapper;
using MathTutor.Core.Entities;
using MathTutor.Core.Models;
using MathTutor.Core.Models.Auth;

namespace MathTutor.Core.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // User mapping
        CreateMap<ApplicationUser, UserModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.LastLogin, opt => opt.MapFrom(src => src.LastLogin))
            .ForMember(dest => dest.IsVerified, opt => opt.MapFrom(src => src.IsVerified));

        // Math Category mappings
        CreateMap<MathCategory, MathCategoryModel>()
            .ForMember(dest => dest.Topics, opt => opt.MapFrom(src => src.Topics));
        CreateMap<MathCategoryModel, MathCategory>();

        // Math Topic mappings
        CreateMap<MathTopic, MathTopicModel>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
            .ForMember(dest => dest.ProblemCount, opt => opt.MapFrom(src => src.Problems.Count));
        CreateMap<MathTopicModel, MathTopic>();

        // Math Problem mappings
        CreateMap<MathProblem, MathProblemModel>()
            .ForMember(dest => dest.TopicName, opt => opt.MapFrom(src => src.Topic.Name));
        CreateMap<MathProblemModel, MathProblem>();

        // Math Problem Attempt mappings
        CreateMap<MathProblemAttempt, MathProblemAttemptModel>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
            .ForMember(dest => dest.ProblemStatement, opt => opt.MapFrom(src => src.Problem.Statement));
        CreateMap<MathProblemAttemptModel, MathProblemAttempt>();

        // Student Progress mappings
        CreateMap<StudentProgress, StudentProgressModel>()
            .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
            .ForMember(dest => dest.TopicName, opt => opt.MapFrom(src => src.Topic.Name));
        CreateMap<StudentProgressModel, StudentProgress>();
        
        // User Math Problem mappings
        CreateMap<UserMathProblem, UserMathProblemModel>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"));
        CreateMap<UserMathProblemModel, UserMathProblem>();
    }
} 
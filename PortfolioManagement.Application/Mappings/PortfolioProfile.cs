using AutoMapper;
using PortfolioManagement.Application.DTOs;
using PortfolioManagement.Domain.Entities;

namespace PortfolioManagement.Application.Mappings;

public sealed class PortfolioProfile : Profile
{
    public PortfolioProfile()
    {
        // Entity → DTO (read direction only — no ReverseMap)
        // ReverseMap creates Dto → Entity maps that include Id, which can
        // accidentally overwrite entity keys during updates.
        CreateMap<AboutMe, AboutMeDto>();
        CreateMap<AboutMeRequest, AboutMe>();

        CreateMap<Skill, SkillDto>();
        CreateMap<SkillRequest, Skill>();

        CreateMap<Project, ProjectDto>();
        CreateMap<ProjectRequest, Project>();

        CreateMap<Experience, ExperienceDto>();
        CreateMap<ExperienceRequest, Experience>();

        CreateMap<Education, EducationDto>();
        CreateMap<EducationRequest, Education>();

        CreateMap<Certificate, CertificateDto>();
        CreateMap<CertificateRequest, Certificate>();

        CreateMap<ContactMessage, ContactMessageDto>();
        CreateMap<ContactMessageRequest, ContactMessage>();

        CreateMap<Testimonial, TestimonialDto>();
        CreateMap<TestimonialRequest, Testimonial>();
    }
}

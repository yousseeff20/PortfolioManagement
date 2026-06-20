using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PortfolioManagement.Application.Behaviors;
using PortfolioManagement.Application.Common;
using PortfolioManagement.Application.Common.Cqrs;
using PortfolioManagement.Application.DTOs;
using PortfolioManagement.Domain.Entities;

namespace PortfolioManagement.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(DependencyInjection).Assembly);
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        RegisterCrud<AboutMe, AboutMeDto, AboutMeRequest>(services);
        RegisterCrud<Skill, SkillDto, SkillRequest>(services);
        RegisterCrud<Project, ProjectDto, ProjectRequest>(services);
        RegisterCrud<Experience, ExperienceDto, ExperienceRequest>(services);
        RegisterCrud<Education, EducationDto, EducationRequest>(services);
        RegisterCrud<Certificate, CertificateDto, CertificateRequest>(services);
        RegisterCrud<ContactMessage, ContactMessageDto, ContactMessageRequest>(services);
        RegisterCrud<Testimonial, TestimonialDto, TestimonialRequest>(services);

        return services;
    }

    private static void RegisterCrud<TEntity, TDto, TRequest>(IServiceCollection services)
        where TEntity : PortfolioManagement.Domain.Common.BaseEntity
    {
        services.AddScoped<IRequestHandler<GetPagedQuery<TEntity, TDto>, Result<PagedResult<TDto>>>, GetPagedQueryHandler<TEntity, TDto>>();
        services.AddScoped<IRequestHandler<GetByIdQuery<TEntity, TDto>, Result<TDto>>, GetByIdQueryHandler<TEntity, TDto>>();
        services.AddScoped<IRequestHandler<CreateCommand<TEntity, TRequest, TDto>, Result<TDto>>, CreateCommandHandler<TEntity, TRequest, TDto>>();
        services.AddScoped<IRequestHandler<UpdateCommand<TEntity, TRequest, TDto>, Result<TDto>>, UpdateCommandHandler<TEntity, TRequest, TDto>>();
        services.AddScoped<IRequestHandler<DeleteCommand<TEntity>, Result>, DeleteCommandHandler<TEntity>>();
    }
}

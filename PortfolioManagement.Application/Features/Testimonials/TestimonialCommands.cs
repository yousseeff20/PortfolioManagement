using MediatR;
using PortfolioManagement.Application.Common;
using PortfolioManagement.Application.DTOs;
using PortfolioManagement.Domain.Entities;

namespace PortfolioManagement.Application.Features.Testimonials;

public sealed record SubmitTestimonialCommand(TestimonialRequest Request) : IRequest<Result<TestimonialDto>>;
public sealed record ReviewTestimonialCommand(Guid Id, bool Approved) : IRequest<Result<TestimonialDto>>;
public sealed record GetApprovedTestimonialsQuery : IRequest<Result<IReadOnlyList<TestimonialDto>>>;

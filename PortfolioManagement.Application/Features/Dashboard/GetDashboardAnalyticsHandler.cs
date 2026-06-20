using MediatR;
using Microsoft.EntityFrameworkCore;
using PortfolioManagement.Application.Common;
using PortfolioManagement.Application.DTOs;
using PortfolioManagement.Application.Interfaces;
using PortfolioManagement.Domain.Entities;
using PortfolioManagement.Domain.Enums;

namespace PortfolioManagement.Application.Features.Dashboard;

public sealed class GetDashboardAnalyticsHandler : IRequestHandler<GetDashboardAnalyticsQuery, Result<DashboardAnalyticsDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetDashboardAnalyticsHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Result<DashboardAnalyticsDto>> Handle(GetDashboardAnalyticsQuery request, CancellationToken cancellationToken)
    {
        var dto = new DashboardAnalyticsDto(
            await _unitOfWork.Repository<Project>().Query().CountAsync(cancellationToken),
            await _unitOfWork.Repository<Skill>().Query().CountAsync(cancellationToken),
            await _unitOfWork.Repository<Certificate>().Query().CountAsync(cancellationToken),
            await _unitOfWork.Repository<Testimonial>().Query().CountAsync(cancellationToken),
            await _unitOfWork.Repository<Testimonial>().Query().CountAsync(t => t.Status == TestimonialStatus.Pending, cancellationToken),
            await _unitOfWork.Repository<ContactMessage>().Query().CountAsync(cancellationToken));

        return Result<DashboardAnalyticsDto>.Success(dto);
    }
}

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PortfolioManagement.Application.Common;
using PortfolioManagement.Application.DTOs;
using PortfolioManagement.Application.Interfaces;
using PortfolioManagement.Domain.Entities;
using PortfolioManagement.Domain.Enums;

namespace PortfolioManagement.Application.Features.Testimonials;

public sealed class SubmitTestimonialCommandHandler : IRequestHandler<SubmitTestimonialCommand, Result<TestimonialDto>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public SubmitTestimonialCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<TestimonialDto>> Handle(SubmitTestimonialCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<Testimonial>(request.Request);
        entity.Status = TestimonialStatus.Pending;
        await _unitOfWork.Repository<Testimonial>().AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<TestimonialDto>.Success(_mapper.Map<TestimonialDto>(entity));
    }
}

public sealed class ReviewTestimonialCommandHandler : IRequestHandler<ReviewTestimonialCommand, Result<TestimonialDto>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public ReviewTestimonialCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<TestimonialDto>> Handle(ReviewTestimonialCommand request, CancellationToken cancellationToken)
    {
        var entity = await _unitOfWork.Repository<Testimonial>().GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
        {
            return Result<TestimonialDto>.Failure("NotFound", "Testimonial was not found.");
        }

        entity.Status = request.Approved ? TestimonialStatus.Approved : TestimonialStatus.Rejected;
        entity.ReviewedAt = DateTimeOffset.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<TestimonialDto>.Success(_mapper.Map<TestimonialDto>(entity));
    }
}

public sealed class GetApprovedTestimonialsHandler : IRequestHandler<GetApprovedTestimonialsQuery, Result<IReadOnlyList<TestimonialDto>>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public GetApprovedTestimonialsHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyList<TestimonialDto>>> Handle(GetApprovedTestimonialsQuery request, CancellationToken cancellationToken)
    {
        var testimonials = await _unitOfWork.Repository<Testimonial>().Query()
            .Where(x => x.Status == TestimonialStatus.Approved)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<TestimonialDto>>.Success(_mapper.Map<IReadOnlyList<TestimonialDto>>(testimonials));
    }
}

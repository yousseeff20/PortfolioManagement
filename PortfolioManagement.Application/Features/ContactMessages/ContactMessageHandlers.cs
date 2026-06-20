using AutoMapper;
using MediatR;
using PortfolioManagement.Application.Common;
using PortfolioManagement.Application.DTOs;
using PortfolioManagement.Application.Interfaces;
using PortfolioManagement.Domain.Entities;

namespace PortfolioManagement.Application.Features.ContactMessages;

public sealed class SubmitContactMessageCommandHandler : IRequestHandler<SubmitContactMessageCommand, Result<ContactMessageDto>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public SubmitContactMessageCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<ContactMessageDto>> Handle(SubmitContactMessageCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<ContactMessage>(request.Request);
        await _unitOfWork.Repository<ContactMessage>().AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<ContactMessageDto>.Success(_mapper.Map<ContactMessageDto>(entity));
    }
}

public sealed class MarkContactMessageReadCommandHandler : IRequestHandler<MarkContactMessageReadCommand, Result<ContactMessageDto>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public MarkContactMessageReadCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<ContactMessageDto>> Handle(MarkContactMessageReadCommand request, CancellationToken cancellationToken)
    {
        var entity = await _unitOfWork.Repository<ContactMessage>().GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
        {
            return Result<ContactMessageDto>.Failure("NotFound", "Contact message was not found.");
        }

        entity.IsRead = true;
        entity.ReadAt = DateTimeOffset.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<ContactMessageDto>.Success(_mapper.Map<ContactMessageDto>(entity));
    }
}

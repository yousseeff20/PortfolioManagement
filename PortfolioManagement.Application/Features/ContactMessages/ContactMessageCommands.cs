using MediatR;
using PortfolioManagement.Application.Common;
using PortfolioManagement.Application.DTOs;

namespace PortfolioManagement.Application.Features.ContactMessages;

public sealed record SubmitContactMessageCommand(ContactMessageRequest Request) : IRequest<Result<ContactMessageDto>>;
public sealed record MarkContactMessageReadCommand(Guid Id) : IRequest<Result<ContactMessageDto>>;

using FluentValidation;
using PortfolioManagement.Application.Features.ContactMessages;
using PortfolioManagement.Application.Features.Testimonials;

namespace PortfolioManagement.Application.Validators;

public sealed class SubmitContactMessageCommandValidator : AbstractValidator<SubmitContactMessageCommand>
{
    public SubmitContactMessageCommandValidator()
    {
        RuleFor(x => x.Request.Name).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Request.Email).NotEmpty().EmailAddress().MaximumLength(180);
        RuleFor(x => x.Request.Subject).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Request.Message).NotEmpty().MaximumLength(4000);
    }
}

public sealed class SubmitTestimonialCommandValidator : AbstractValidator<SubmitTestimonialCommand>
{
    public SubmitTestimonialCommandValidator()
    {
        RuleFor(x => x.Request.Name).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Request.JobTitle).NotEmpty().MaximumLength(160);
        RuleFor(x => x.Request.Comment).NotEmpty().MaximumLength(1500);
        RuleFor(x => x.Request.Rating).InclusiveBetween(1, 5);
    }
}

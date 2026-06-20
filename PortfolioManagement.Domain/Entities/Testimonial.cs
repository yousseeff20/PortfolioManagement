using PortfolioManagement.Domain.Common;
using PortfolioManagement.Domain.Enums;

namespace PortfolioManagement.Domain.Entities;

public sealed class Testimonial : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }
    public string Comment { get; set; } = string.Empty;
    public int Rating { get; set; }
    public TestimonialStatus Status { get; set; } = TestimonialStatus.Pending;
    public DateTimeOffset? ReviewedAt { get; set; }
}

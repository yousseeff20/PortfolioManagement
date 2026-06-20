using PortfolioManagement.Domain.Common;

namespace PortfolioManagement.Domain.Entities;

public sealed class Experience : BaseEntity
{
    public string Company { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string Description { get; set; } = string.Empty;
}

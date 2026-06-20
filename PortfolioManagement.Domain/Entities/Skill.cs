using PortfolioManagement.Domain.Common;

namespace PortfolioManagement.Domain.Entities;

public sealed class Skill : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public int Percentage { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public int DisplayOrder { get; set; }
}

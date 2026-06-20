using PortfolioManagement.Domain.Common;

namespace PortfolioManagement.Domain.Entities;

public sealed class Certificate : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public DateOnly IssueDate { get; set; }
    public string? CredentialUrl { get; set; }
    public string? CertificateImageUrl { get; set; }
}

using PortfolioManagement.Domain.Common;

namespace PortfolioManagement.Domain.Entities;

public sealed class Project : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Technologies { get; set; } = [];
    public string? GitHubUrl { get; set; }
    public string? LiveDemoUrl { get; set; }
    public string? ThumbnailUrl { get; set; }
    public List<string> GalleryImageUrls { get; set; } = [];
    public bool FeaturedProject { get; set; }
    public DateTimeOffset ProjectCreatedDate { get; set; } = DateTimeOffset.UtcNow;
}

using PortfolioManagement.Domain.Enums;

namespace PortfolioManagement.Application.DTOs;

public sealed record FileUploadResponse(string FileName, string Url, long Size, string ContentType);

public sealed record AboutMeDto(Guid Id, string FullName, string Title, string Bio, string? ProfileImageUrl, string? CvFileUrl, string Location, string Email, string? Phone, string? LinkedInUrl, string? GitHubUrl);
public sealed record AboutMeRequest(string FullName, string Title, string Bio, string? ProfileImageUrl, string? CvFileUrl, string Location, string Email, string? Phone, string? LinkedInUrl, string? GitHubUrl);

public sealed record SkillDto(Guid Id, string Name, int Percentage, string Category, string? Icon, int DisplayOrder);
public sealed record SkillRequest(string Name, int Percentage, string Category, string? Icon, int DisplayOrder);

public sealed record ProjectDto(Guid Id, string Title, string Description, List<string> Technologies, string? GitHubUrl, string? LiveDemoUrl, string? ThumbnailUrl, List<string> GalleryImageUrls, bool FeaturedProject, DateTimeOffset ProjectCreatedDate);
public sealed record ProjectRequest(string Title, string Description, List<string> Technologies, string? GitHubUrl, string? LiveDemoUrl, string? ThumbnailUrl, List<string> GalleryImageUrls, bool FeaturedProject, DateTimeOffset ProjectCreatedDate);

public sealed record ExperienceDto(Guid Id, string Company, string Position, DateOnly StartDate, DateOnly? EndDate, string Description);
public sealed record ExperienceRequest(string Company, string Position, DateOnly StartDate, DateOnly? EndDate, string Description);

public sealed record EducationDto(Guid Id, string Institution, string Degree, DateOnly StartDate, DateOnly? EndDate, string Description);
public sealed record EducationRequest(string Institution, string Degree, DateOnly StartDate, DateOnly? EndDate, string Description);

public sealed record CertificateDto(Guid Id, string Title, string Issuer, DateOnly IssueDate, string? CredentialUrl, string? CertificateImageUrl);
public sealed record CertificateRequest(string Title, string Issuer, DateOnly IssueDate, string? CredentialUrl, string? CertificateImageUrl);

public sealed record ContactMessageDto(Guid Id, string Name, string Email, string Subject, string Message, bool IsRead, DateTimeOffset CreatedAt);
public sealed record ContactMessageRequest(string Name, string Email, string Subject, string Message);

public sealed record TestimonialDto(Guid Id, string Name, string JobTitle, string? ProfileImageUrl, string Comment, int Rating, TestimonialStatus Status, DateTimeOffset CreatedAt);
public sealed record TestimonialRequest(string Name, string JobTitle, string? ProfileImageUrl, string Comment, int Rating);

public sealed record DashboardAnalyticsDto(int TotalProjects, int TotalSkills, int TotalCertificates, int TotalTestimonials, int PendingTestimonials, int TotalMessages);

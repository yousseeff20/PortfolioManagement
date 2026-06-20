namespace PortfolioManagement.Application.Interfaces;

public sealed record StoredFile(string FileName, string Url, long Size, string ContentType);

public interface IFileStorageService
{
    Task<StoredFile> SaveAsync(Stream stream, string fileName, string contentType, CancellationToken cancellationToken = default);
    Task DeleteAsync(string url, CancellationToken cancellationToken = default);
}

using Microsoft.Extensions.Options;
using PortfolioManagement.Application.Interfaces;
using PortfolioManagement.Infrastructure.Options;

namespace PortfolioManagement.Infrastructure.Services;

public sealed class LocalFileStorageService : IFileStorageService
{
    private readonly LocalStorageOptions _options;

    public LocalFileStorageService(IOptions<LocalStorageOptions> options)
    {
        _options = options.Value;
    }

    public async Task<StoredFile> SaveAsync(Stream stream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(_options.RootPath);
        var extension = Path.GetExtension(fileName);
        var safeName = $"{Guid.NewGuid():N}{extension}";
        var path = Path.Combine(_options.RootPath, safeName);

        await using var output = File.Create(path);
        await stream.CopyToAsync(output, cancellationToken);

        var url = $"{_options.PublicBasePath.TrimEnd('/')}/{safeName}";
        return new StoredFile(safeName, url, output.Length, contentType);
    }

    public Task DeleteAsync(string url, CancellationToken cancellationToken = default)
    {
        var fileName = Path.GetFileName(url);
        var path = Path.Combine(_options.RootPath, fileName);
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        return Task.CompletedTask;
    }
}

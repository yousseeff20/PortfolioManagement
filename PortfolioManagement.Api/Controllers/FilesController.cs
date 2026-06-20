using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioManagement.Application.DTOs;
using PortfolioManagement.Application.Interfaces;
using PortfolioManagement.Domain.Constants;

namespace PortfolioManagement.Api.Controllers;

[Authorize(Roles = AppRoles.Admin)]
[Route("api/files")]
public sealed class FilesController : ApiControllerBase
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg", ".ico",
        ".pdf", ".doc", ".docx"
    };

    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg", "image/png", "image/gif", "image/webp", "image/svg+xml", "image/x-icon",
        "application/pdf", "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
    };

    private readonly IFileStorageService _fileStorageService;

    public FilesController(IFileStorageService fileStorageService) => _fileStorageService = fileStorageService;

    [HttpPost]
    [RequestSizeLimit(20_000_000)]
    public async Task<ActionResult<FileUploadResponse>> Upload(IFormFile file, CancellationToken cancellationToken)
    {
        if (file.Length == 0)
        {
            return BadRequest("File is empty.");
        }

        var extension = Path.GetExtension(file.FileName);
        if (!AllowedExtensions.Contains(extension))
        {
            return BadRequest($"File type '{extension}' is not allowed.");
        }

        if (!AllowedContentTypes.Contains(file.ContentType))
        {
            return BadRequest($"Content type '{file.ContentType}' is not allowed.");
        }

        await using var stream = file.OpenReadStream();
        var stored = await _fileStorageService.SaveAsync(stream, file.FileName, file.ContentType, cancellationToken);
        return Ok(new FileUploadResponse(stored.FileName, stored.Url, stored.Size, stored.ContentType));
    }
}

using Microsoft.AspNetCore.Mvc;
using PortfolioManagement.Application.Common;

namespace PortfolioManagement.Api.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult FromResult(Result result)
    {
        if (result.Succeeded)
        {
            return NoContent();
        }

        return Problem(result.Error?.Message, statusCode: ToStatusCode(result.Error?.Code), title: result.Error?.Code);
    }

    protected ActionResult<T> FromResult<T>(Result<T> result)
    {
        if (result.Succeeded && result.Value is not null)
        {
            return Ok(result.Value);
        }

        return Problem(result.Error?.Message, statusCode: ToStatusCode(result.Error?.Code), title: result.Error?.Code);
    }

    private static int ToStatusCode(string? code) => code switch
    {
        "NotFound" => StatusCodes.Status404NotFound,
        "Forbidden" => StatusCodes.Status403Forbidden,
        "InvalidCredentials" or "InvalidRefreshToken" => StatusCodes.Status401Unauthorized,
        _ => StatusCodes.Status400BadRequest
    };
}

using Microsoft.AspNetCore.Diagnostics;

namespace Sellow.Api.Exceptions;

internal sealed class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        var result = Results.Problem(
            statusCode: StatusCodes.Status500InternalServerError,
            title: "An unexpected error occurred.",
            detail: "Please try again later."
        );

        await result.ExecuteAsync(httpContext);

        return true;
    }
}

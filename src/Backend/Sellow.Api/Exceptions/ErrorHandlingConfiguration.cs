using System.Diagnostics;

namespace Sellow.Api.Exceptions;

internal static class ErrorHandlingConfiguration
{
    public static IServiceCollection AddApiErrorHandling(this IServiceCollection services)
        => services.AddProblemDetails(options =>
            {
                options.CustomizeProblemDetails = context =>
                {
                    context.ProblemDetails.Extensions["traceId"] =
                        Activity.Current?.TraceId.ToString()
                        ?? context.HttpContext.TraceIdentifier;
                };
            })
            .AddExceptionHandler<GlobalExceptionHandler>();
}

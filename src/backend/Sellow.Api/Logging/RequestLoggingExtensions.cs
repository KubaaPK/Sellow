using System.Diagnostics;
using Serilog;
using Serilog.Context;

namespace Sellow.Api.Logging;

internal static class RequestLoggingExtensions
{
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
    {
        app.Use(async (context, next) =>
        {
            var traceId = Activity.Current?.Id ?? context.TraceIdentifier;

            using (LogContext.PushProperty("RequestTraceId", traceId))
            {
                await next(context);
            }
        });

        app.UseSerilogRequestLogging();

        return app;
    }
}
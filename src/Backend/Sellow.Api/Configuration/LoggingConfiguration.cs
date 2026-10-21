using Serilog;
using Serilog.Formatting.Compact;

namespace Sellow.Api.Configuration;

internal static class LoggingConfiguration
{
    public static IServiceCollection AddApiLogging(this IServiceCollection services, IConfiguration configuration,
        IHostEnvironment environment)
        => services.AddSerilog((sp, options) =>
        {
            options
                .ReadFrom.Configuration(configuration)
                .ReadFrom.Services(sp)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "Sellow.Api")
                .Enrich.WithProperty("Environment", environment.EnvironmentName);

            if (environment.IsDevelopment())
            {
                options.WriteTo.Console(outputTemplate:
                    "[{Timestamp:HH:mm:ss} {Level:u3}] "
                    + "{SourceContext} "
                    + "[TraceId: {TraceId} SpanId: {SpanId}] "
                    + "{Message:lj}{NewLine}{Exception}");
            }
            else
            {
                options.WriteTo.Console(new CompactJsonFormatter());
            }
        }, preserveStaticLogger: true);
}

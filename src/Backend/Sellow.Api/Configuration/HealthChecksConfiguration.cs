using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Sellow.Api.Configuration;

internal static class HealthChecksConfiguration
{
    public static IServiceCollection AddApiHealthChecks(this IServiceCollection services)
        => services
            .AddOptions<ConnectionStringsOptions>()
            .BindConfiguration(ConnectionStringsOptions.SectionName)
            .Validate(options => !string.IsNullOrWhiteSpace(options.Sellow), "ConnectionStrings:Sellow is required.`")
            .ValidateOnStart()
            .Services
            .AddHealthChecks()
            .AddNpgSql(connectionStringFactory: provider => provider
                    .GetRequiredService<IOptions<ConnectionStringsOptions>>()
                    .Value.Sellow,
                name: "postgresql",
                tags: ["ready"],
                timeout: TimeSpan.FromSeconds(5))
            .Services;

    public static IEndpointRouteBuilder MapApiHealthChecks(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = _ => false
        });

        endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = registration => registration.Tags.Contains("ready")
        });

        return endpoints;
    }
}

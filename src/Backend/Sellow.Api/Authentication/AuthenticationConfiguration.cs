using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Sellow.Api.Authentication;

internal static class AuthenticationConfiguration
{
    private static readonly TimeSpan TokenClockSkew = TimeSpan.FromSeconds(30);

    public static IServiceCollection AddApiAuthentication(this IServiceCollection services)
        => services
            .AddOptions<FirebaseOptions>()
            .BindConfiguration(FirebaseOptions.SectionName)
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.ProjectId) &&
                           options.ProjectId.All(character => char.IsAsciiLetterOrDigit(character) || character == '-'),
                "Firebase:ProjectId must be a valid project identifier.")
            .ValidateOnStart()
            .Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer()
            .Services
            .AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<FirebaseOptions>>((options, firebase) =>
            {
                var projectId = firebase.Value.ProjectId;
                var issuer = $"https://securetoken.google.com/{projectId}";

                options.Authority = issuer;
                options.Audience = projectId;
                options.RequireHttpsMetadata = true;
                options.MapInboundClaims = false;
                options.SaveToken = false;
                options.IncludeErrorDetails = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = projectId,
                    IgnoreTrailingSlashWhenValidatingAudience = false,
                    ValidateLifetime = true,
                    RequireExpirationTime = true,
                    RequireSignedTokens = true,
                    ValidateIssuerSigningKey = true,
                    ValidAlgorithms = [SecurityAlgorithms.RsaSha256],
                    ClockSkew = TokenClockSkew,
                    NameClaimType = "sub"
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = ValidateFirebaseClaims
                };
            })
            .Services
            .AddAuthorization();

    private static Task ValidateFirebaseClaims(TokenValidatedContext context)
    {
        var subject = context.Principal?.FindFirst("sub")?.Value;

        if (string.IsNullOrWhiteSpace(subject) || subject.Length > 128)
        {
            context.Fail("Invalid Firebase subject.");
            return Task.CompletedTask;
        }

        var latestAllowedTime = DateTimeOffset.UtcNow
            .Add(TokenClockSkew)
            .ToUnixTimeSeconds();

        foreach (var claimType in new[] { "iat", "auth_time" })
        {
            var value = context.Principal?.FindFirst(claimType)?.Value;

            if (!long.TryParse(value, out var timestamp) || timestamp < 0 || timestamp > latestAllowedTime)
            {
                context.Fail("Invalid Firebase token timestamps.");
                return Task.CompletedTask;
            }
        }

        return Task.CompletedTask;
    }
}

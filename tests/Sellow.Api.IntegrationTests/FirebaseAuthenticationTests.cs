using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace Sellow.Api.IntegrationTests;

public sealed class FirebaseAuthenticationTests
{
    private const string ProjectId = "sellow-tests";
    private const string Issuer = "https://securetoken.google.com/sellow-tests";
    private const string UserId = "firebase-user-id-123";

    [Theory]
    [InlineData("valid", true)]
    [InlineData("expired", false)]
    [InlineData("wrong-issuer", false)]
    [InlineData("wrong-audience", false)]
    [InlineData("wrong-signature", false)]
    [InlineData("missing-subject", false)]
    [InlineData("missing-auth-time", false)]
    [InlineData("future-issued-at", false)]
    [InlineData("future-auth-time", false)]
    public async Task Authenticate_ValidatesToken(string scenario, bool expectedSuccess)
    {
        using var trustedRsa = RSA.Create(2048);
        using var untrustedRsa = RSA.Create(2048);

        var signingKey = new RsaSecurityKey(trustedRsa)
        {
            KeyId = "test-key"
        };

        var publicKey = new RsaSecurityKey(trustedRsa.ExportParameters(false))
        {
            KeyId = signingKey.KeyId
        };

        var untrustedKey = new RsaSecurityKey(untrustedRsa)
        {
            KeyId = signingKey.KeyId
        };

        var metadata = new OpenIdConnectConfiguration
        {
            Issuer = Issuer
        };

        metadata.SigningKeys.Add(publicKey);

        await using var baseFactory = new ApiFactory();

        await using var factory = baseFactory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
                services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme,
                    options => options.ConfigurationManager =
                        new StaticConfigurationManager<OpenIdConnectConfiguration>(metadata)));
        });

        var token = CreateToken(scenario, scenario == "wrong-signature" ? untrustedKey : signingKey);

        using var scope = factory.Services.CreateScope();

        var context = new DefaultHttpContext
        {
            RequestServices = scope.ServiceProvider,
            Request =
            {
                Scheme = "https",
                Headers =
                {
                    Authorization = $"Bearer {token}"
                }
            }
        };

        var result = await context.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);

        Assert.Equal(expectedSuccess, result.Succeeded);

        if (expectedSuccess)
        {
            Assert.NotNull(result.Principal);
            Assert.Equal(UserId, result.Principal.FindFirst("sub")?.Value);
            Assert.Equal(UserId, result.Principal.Identity?.Name);
        }
        else
        {
            Assert.NotNull(result.Failure);
            Assert.Null(result.Principal);
        }
    }

    private static string CreateToken(string scenario, SecurityKey signingKey)
    {
        var now = DateTimeOffset.UtcNow;

        var issuedAt = scenario == "future-issued-at"
            ? now.AddMinutes(10)
            : now.AddMinutes(-5);

        var authTime = scenario == "future-auth-time"
            ? now.AddMinutes(10)
            : now.AddMinutes(-5);

        var claims = new List<Claim>
        {
            NumericDateClaim("iat", issuedAt)
        };

        if (scenario != "missing-subject")
        {
            claims.Add(new Claim("sub", UserId));
        }

        if (scenario != "missing-auth-time")
        {
            claims.Add(NumericDateClaim("auth_time", authTime));
        }

        var token = new JwtSecurityToken(
            issuer: scenario == "wrong-issuer"
                ? "https://securetoken.google.com/another-project"
                : Issuer,
            audience: scenario == "wrong-audience"
                ? "another-project"
                : ProjectId,
            claims: claims,
            notBefore: now.AddMinutes(-10).UtcDateTime,
            expires: scenario == "expired"
                ? now.AddMinutes(-2).UtcDateTime
                : now.AddMinutes(10).UtcDateTime,
            signingCredentials: new SigningCredentials(
                signingKey,
                SecurityAlgorithms.RsaSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [Theory]
    [InlineData("missing", HttpStatusCode.Unauthorized)]
    [InlineData("invalid", HttpStatusCode.Unauthorized)]
    [InlineData("valid", HttpStatusCode.OK)]
    public async Task ProtectedEndpoint_EnforcesAuthentication(
        string scenario,
        HttpStatusCode expectedStatus)
    {
        using var rsa = RSA.Create(2048);
        using var otherRsa = RSA.Create(2048);

        var signingKey = new RsaSecurityKey(rsa)
        {
            KeyId = "http-test-key"
        };

        var publicKey = new RsaSecurityKey(rsa.ExportParameters(false))
        {
            KeyId = signingKey.KeyId
        };

        var otherKey = new RsaSecurityKey(otherRsa)
        {
            KeyId = signingKey.KeyId
        };

        var metadata = new OpenIdConnectConfiguration
        {
            Issuer = Issuer
        };

        metadata.SigningKeys.Add(publicKey);

        await using var baseFactory = new ApiFactory();

        await using var factory = baseFactory.WithWebHostBuilder(builder =>
            builder.ConfigureTestServices(services =>
                services.PostConfigure<JwtBearerOptions>(
                    JwtBearerDefaults.AuthenticationScheme,
                    options =>
                    {
                        options.ConfigurationManager =
                            new StaticConfigurationManager<
                                OpenIdConnectConfiguration>(metadata);
                    }).AddSingleton<IStartupFilter>(
                    new ProtectedEndpointStartupFilter())
            )
        );

        using var client = factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("https://localhost"),
                AllowAutoRedirect = false
            });

        using var request = new HttpRequestMessage(HttpMethod.Get, "/__tests/protected");

        if (scenario != "missing")
        {
            var token = CreateToken("valid", scenario == "invalid" ? otherKey : signingKey);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        using var response = await client.SendAsync(request);

        Assert.Equal(expectedStatus, response.StatusCode);

        if (expectedStatus == HttpStatusCode.OK)
        {
            Assert.Equal(UserId, await response.Content.ReadAsStringAsync());
        }
        else
        {
            Assert.Contains(response.Headers.WwwAuthenticate, value => value.Scheme == "Bearer");
        }
    }


    private static Claim NumericDateClaim(string type, DateTimeOffset value)
        => new(type, value.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture), ClaimValueTypes.Integer64);

    private sealed class ProtectedEndpointStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(
            Action<IApplicationBuilder> next)
        {
            return app =>
            {
                app.UseRouting();

                next(app);

                app.UseEndpoints(endpoints =>
                {
                    endpoints
                        .MapGet(
                            "/__tests/protected",
                            (ClaimsPrincipal user) =>
                                Results.Text(user.FindFirst("sub")!.Value))
                        .RequireAuthorization();
                });
            };
        }
    }
}



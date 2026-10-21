using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Sellow.Api.IntegrationTests;

public sealed class GlobalExceptionHandlerTests
{
    private const string FailurePath = "/__tests/unhandled-exception";
    private const string SensitiveMessage = "Internal credential: test-secret";

    [Theory]
    [InlineData("Development")]
    [InlineData("Production")]
    public async Task UnhandledException_ReturnsSafeProblemDetails(string environment)
    {
        using var factory = new ApiFactory()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment(environment);

                builder.ConfigureServices(services =>
                {
                    services.AddSingleton<IStartupFilter>(new ThrowingStartupFilter());
                });
            });

        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
            AllowAutoRedirect = false
        });

        using var response = await client.GetAsync(FailurePath);
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var document = JsonDocument.Parse(body);
        var problem = document.RootElement;

        Assert.Equal(500, problem.GetProperty("status").GetInt32());
        Assert.Equal("An unexpected error occurred.", problem.GetProperty("title").GetString());

        Assert.Equal("Please try again later.", problem.GetProperty("detail").GetString());

        Assert.False(string.IsNullOrWhiteSpace(problem.GetProperty("traceId").GetString()));

        Assert.DoesNotContain(SensitiveMessage, body);
        Assert.DoesNotContain(nameof(InvalidOperationException), body);
        Assert.DoesNotContain(nameof(ThrowingStartupFilter), body);
    }

    private sealed class ThrowingStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
            => app =>
            {
                next(app);

                app.Use((context, nextMiddleware) => context.Request.Path == FailurePath
                    ? throw new InvalidOperationException(SensitiveMessage)
                    : nextMiddleware(context));
            };
    }
}

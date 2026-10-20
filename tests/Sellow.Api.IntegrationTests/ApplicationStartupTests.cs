using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Sellow.Api.IntegrationTests;

public sealed class ApplicationStartupTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>

{
    [Fact]
    public async Task UnknownRoute_ReturnsNotFound()
    {
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
            AllowAutoRedirect = false
        });

        using var response = await client.GetAsync("/__tests/nonexistent-route");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}

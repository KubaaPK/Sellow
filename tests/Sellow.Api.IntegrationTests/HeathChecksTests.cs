using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Sellow.Api.IntegrationTests;

public sealed class HealthChecksTests(ApiFactory factory)
    : IClassFixture<ApiFactory>
{
    [Theory]
    [InlineData("/health/live", HttpStatusCode.OK, "Healthy")]
    [InlineData(
        "/health/ready",
        HttpStatusCode.ServiceUnavailable,
        "Unhealthy")]
    public async Task DatabaseUnavailable_ReturnsExpectedHealthStatus(
        string path,
        HttpStatusCode expectedStatus,
        string expectedBody)
    {
        using var client = factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("https://localhost"),
                AllowAutoRedirect = false
            });

        using var response = await client.GetAsync(path);
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(expectedStatus, response.StatusCode);
        Assert.Equal(expectedBody, body);
    }
}

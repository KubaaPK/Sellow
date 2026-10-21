using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Sellow.Api.IntegrationTests;

public sealed class OpenApiTests
{
    [Theory]
    [InlineData("Development", HttpStatusCode.OK)]
    [InlineData("Production", HttpStatusCode.NotFound)]
    public async Task Document_IsAvailableOnlyInDevelopment(string environment, HttpStatusCode expectedStatus)
    {
        using var factory = new ApiFactory()
            .WithWebHostBuilder(builder => builder.UseEnvironment(environment));

        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
            AllowAutoRedirect = false
        });

        using var response = await client.GetAsync("/openapi/v1.json");

        Assert.Equal(expectedStatus, response.StatusCode);

        if (expectedStatus == HttpStatusCode.OK)
        {
            using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

            Assert.True(document.RootElement.TryGetProperty("openapi", out _));
            Assert.True(document.RootElement.TryGetProperty("paths", out _));
        }
    }
}

using System.Net;
using System.Text.Json;

namespace Sellow.Api.IntegrationTests;

public sealed class ProgramTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly ApiWebApplicationFactory _factory;

    public ProgramTests(ApiWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetApiV1_Returns200AndGreeting()
    {
        // Arrange
        using var client = _factory.CreateClient();

        // Act
        using var response = await client.GetAsync("/api/v1");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Sellow API v1", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task GetRoot_Returns404()
    {
        // Arrange
        using var client = _factory.CreateClient();

        // Act
        using var response = await client.GetAsync("/");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task GetThrow_Returns500ProblemDetailsWithoutExceptionDetails()
    {
        // Arrange
        using var client = _factory.CreateClient();

        // Act
        using var response = await client.GetAsync("/__tests/throw");
        var body = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var document = JsonDocument.Parse(body);
        var problem = document.RootElement;

        Assert.Equal(500, problem.GetProperty("status").GetInt32());
        Assert.Equal("An unexpected error occurred.", problem.GetProperty("title").GetString());
        Assert.Equal("about:blank", problem.GetProperty("type").GetString());

        Assert.True(problem.TryGetProperty("traceId", out var traceId));
        Assert.False(string.IsNullOrWhiteSpace(traceId.GetString()));

        Assert.DoesNotContain("Sensitive test exception message.", body);
        Assert.DoesNotContain("InvalidOperationException", body);
        Assert.DoesNotContain(nameof(ThrowingStartupFilter), body);
    }

    [Fact]
    public async Task GetHealth_Returns200AndHealthyStatus()
    {
        // Arrange
        using var client = _factory.CreateClient();

        // Act
        using var response = await client.GetAsync("/health");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Healthy", await response.Content.ReadAsStringAsync());
    }
    
    [Fact]
    public async Task OpenApi_IsNotAvailableOutsideDevelopment()
    {
        // Arrange
        using var client = _factory.CreateClient();
    
        // Act
        using var response = await client.GetAsync("/openapi/v1.json");
    
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
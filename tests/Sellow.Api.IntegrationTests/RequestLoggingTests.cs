using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Sellow.Api.IntegrationTests.Logging;
using Serilog.Core;
using Serilog.Events;

namespace Sellow.Api.IntegrationTests;

public sealed class RequestLoggingTests
{
    [Fact]
    public async Task Request_WritesOneStructuredSummaryWithTraceIdentifiers()
    {
        var sink = new InMemoryLogSink();

        await using var baseFactory = new ApiFactory();

        await using var factory = baseFactory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services => { services.AddSingleton<ILogEventSink>(sink); });
        });

        using var client = factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("https://localhost"),
                AllowAutoRedirect = false
            });

        var traceId = ActivityTraceId.CreateRandom();
        var parentSpanId = ActivitySpanId.CreateRandom();

        using var request = new HttpRequestMessage(HttpMethod.Get, "/health/live");

        request.Headers.Add("traceparent", $"00-{traceId}-{parentSpanId}-01");

        using var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        await sink.RequestLogged.WaitAsync(TimeSpan.FromSeconds(5));

        var logEvent = Assert.Single(sink.Events, InMemoryLogSink.IsRequestSummary);

        Assert.Equal(LogEventLevel.Information, logEvent.Level);

        Assert.Equal("GET", Assert.IsType<ScalarValue>(logEvent.Properties["RequestMethod"]).Value);

        Assert.Equal("/health/live", Assert.IsType<ScalarValue>(logEvent.Properties["RequestPath"]).Value);

        Assert.Equal(200, Assert.IsType<ScalarValue>(logEvent.Properties["StatusCode"]).Value);

        var elapsed = Assert.IsType<double>(Assert.IsType<ScalarValue>(logEvent.Properties["Elapsed"]).Value);

        Assert.True(elapsed >= 0);

        Assert.True(logEvent.TraceId.HasValue);
        Assert.Equal(traceId, logEvent.TraceId.Value);

        Assert.True(logEvent.SpanId.HasValue);
        Assert.NotEqual(default(ActivitySpanId), logEvent.SpanId.Value);
        Assert.NotEqual(parentSpanId, logEvent.SpanId.Value);
    }
}

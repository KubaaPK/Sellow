using System.Collections.Concurrent;
using Serilog.Core;
using Serilog.Events;

namespace Sellow.Api.IntegrationTests.Logging;

internal sealed class InMemoryLogSink : ILogEventSink
{
    private readonly ConcurrentQueue<LogEvent> _events = new();

    private readonly TaskCompletionSource<bool> _requestLogged =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    public IReadOnlyCollection<LogEvent> Events => _events.ToArray();

    public Task RequestLogged => _requestLogged.Task;

    public void Emit(LogEvent logEvent)
    {
        _events.Enqueue(logEvent);

        if (IsRequestSummary(logEvent))
        {
            _requestLogged.TrySetResult(true);
        }
    }

    public static bool IsRequestSummary(LogEvent logEvent)
    {
        return logEvent.Properties.TryGetValue(
                   "SourceContext",
                   out var source)
               && source is ScalarValue
               {
                   Value: "Serilog.AspNetCore.RequestLoggingMiddleware"
               };
    }
}

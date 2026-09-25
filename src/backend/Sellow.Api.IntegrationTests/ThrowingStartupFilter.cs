using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace Sellow.Api.IntegrationTests;

public sealed class ThrowingStartupFilter : IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        return app =>
        {
            next(app);

            app.Map("/__tests/throw",
                branch =>
                {
                    branch.Run(_ => throw new InvalidOperationException("Sensitive test exception message."));
                });
        };
    }
}
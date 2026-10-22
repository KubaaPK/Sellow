using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Sellow.Api.IntegrationTests;

public sealed class ApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["ConnectionStrings:Sellow"] =
                        "Host=127.0.0.1;Port=1;Database=sellow_tests;"
                        + "Username=sellow_tests;Password=test-only;"
                        + "Timeout=1;Command Timeout=1",
                    ["Firebase:ProjectId"] = "sellow-tests"
                });
        });
    }
}

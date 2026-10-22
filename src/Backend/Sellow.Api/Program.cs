using Asp.Versioning;
using Sellow.Api.Authentication;
using Sellow.Api.Configuration;
using Sellow.Api.Exceptions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApiLogging(builder.Configuration, builder.Environment)
    .AddApiDocumentation()
    .AddApiErrorHandling()
    .AddApiHealthChecks()
    .AddApiAuthentication();

var app = builder.Build();

app.UseSerilogRequestLogging(options => { options.Logger = app.Services.GetRequiredService<Serilog.ILogger>(); });

app.UseExceptionHandler(new ExceptionHandlerOptions
{
    SuppressDiagnosticsCallback = _ => false
});

app
    .UseHttpsRedirection()
    .UseAuthentication()
    .UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().WithDocumentPerVersion();
}

var api = app
    .NewVersionedApi()
    .MapGroup("/api/v{version:apiVersion}")
    .HasApiVersion(new ApiVersion(1, 0));

app.MapApiHealthChecks();

app.Run();

public partial class Program
{
}

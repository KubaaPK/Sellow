using Sellow.Api.Exceptions;
using Sellow.Api.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddProblemDetails()
    .AddExceptionHandler<ErrorHandler>()
    .AddSerilogLogging(builder.Configuration)
    .AddHealthChecks();

var app = builder.Build();

app.UseRequestLogging();
app.UseExceptionHandler();

app.MapGet("/", () => "Hello World!");

app.MapHealthChecks("/health");

app.Run();
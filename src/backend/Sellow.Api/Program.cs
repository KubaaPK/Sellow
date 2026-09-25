using Scalar.AspNetCore;
using Sellow.Api.Exceptions;
using Sellow.Api.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddProblemDetails()
    .AddExceptionHandler<ErrorHandler>()
    .AddSerilogLogging(builder.Configuration)
    .AddHealthChecks()
    .Services
    .AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment()) 
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseRequestLogging();
app.UseExceptionHandler();

app.MapGet("/", () => "Hello World!");

app.MapHealthChecks("/health");

app.Run();
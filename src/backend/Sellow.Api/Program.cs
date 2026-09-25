using Asp.Versioning;
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
    .AddOpenApi()
    .AddApiVersioning(options => 
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
    });
    

var app = builder.Build();

if (app.Environment.IsDevelopment()) 
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseRequestLogging();
app.UseExceptionHandler();

var apiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1.0))
    .ReportApiVersions()
    .Build();

var versionedApi = app
    .MapGroup("/api/v{version:apiVersion}")
    .WithApiVersionSet(apiVersionSet);

versionedApi.MapGet("/", () => "Sellow API v1").MapToApiVersion(1.0);

app.MapHealthChecks("/health");

app.Run();
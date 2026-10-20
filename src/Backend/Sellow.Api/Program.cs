using Asp.Versioning;
using Sellow.Api.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiDocumentation();

var app = builder.Build();

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().WithDocumentPerVersion();
}

var api = app
    .NewVersionedApi()
    .MapGroup("/api/v{version:apiVersion}")
    .HasApiVersion(new ApiVersion(1, 0));

app.Run();

public partial class Program
{
}

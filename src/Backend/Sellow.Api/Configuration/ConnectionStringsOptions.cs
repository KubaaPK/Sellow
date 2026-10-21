namespace Sellow.Api.Configuration;

internal sealed class ConnectionStringsOptions
{
    public const string SectionName = "ConnectionStrings";

    public string Sellow { get; set; } = string.Empty;
}

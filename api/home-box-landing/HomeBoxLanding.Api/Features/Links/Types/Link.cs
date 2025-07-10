namespace HomeBoxLanding.Api.Features.Links.Types;

public class Link
{
    public Guid? Identifier { get; set; }
    public string? Name { get; set; }
    public string? Url { get; set; }
    public string? Host { get; set; }
    public int Port { get; set; }
    public bool IsSecure { get; set; }
    public string? IconUrl { get; set; }
    public string? Category { get; set; }
    public string? SortOrder { get; set; }
}
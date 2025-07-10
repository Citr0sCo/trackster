namespace Trackster.Api.Features.Media.Types;

public class ImportMediaRequest
{
    public ImportType Type { get; set; }
    public string? Username { get; set; }
}
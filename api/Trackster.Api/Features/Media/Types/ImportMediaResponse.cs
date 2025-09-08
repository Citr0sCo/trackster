using Trackster.Api.Core.Types;

namespace Trackster.Api.Features.Media.Types;

public class ImportMediaResponse : CommunicationResponse
{
    public string Data { get; set; }
    public int Total { get; set; }
    public int Processed { get; set; }
    public string Type { get; set; }
}
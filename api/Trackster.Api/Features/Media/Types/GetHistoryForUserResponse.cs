namespace Trackster.Api.Features.Media.Types;

public class GetHistoryForUserResponse
{
    public GetHistoryForUserResponse()
    {
        Media = new List<Media>();
    }
    
    public List<Media> Media { get; set; }
}
namespace Trackster.Api.Features.Media.Types;

public class SearchMediaResponse
{
    public SearchMediaResponse()
    {
        Media = new List<Media>();
    }

    public List<Media> Media { get; set; }
}

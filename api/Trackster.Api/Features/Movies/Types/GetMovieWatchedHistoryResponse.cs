using Trackster.Api.Core.Types;

namespace Trackster.Api.Features.Movies.Types;

public class GetMovieWatchedHistoryResponse : CommunicationResponse
{
    public List<WatchedMovie> WatchHistory { get; set; }
}
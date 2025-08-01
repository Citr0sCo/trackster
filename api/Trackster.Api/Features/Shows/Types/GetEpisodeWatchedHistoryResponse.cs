using Trackster.Api.Core.Types;

namespace Trackster.Api.Features.Shows.Types;

public class GetEpisodeWatchedHistoryResponse : CommunicationResponse
{
    public List<WatchedEpisode> WatchedEpisodes { get; set; }
}
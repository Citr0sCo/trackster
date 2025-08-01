using Trackster.Api.Core.Types;

namespace Trackster.Api.Features.Movies.Types;

public class GetMovieResponse : CommunicationResponse
{
    public Movie Movie { get; set; }
}
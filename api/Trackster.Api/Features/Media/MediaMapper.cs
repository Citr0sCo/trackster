using Trackster.Api.Features.Media.Types;
using MediaResult = Trackster.Api.Features.Media.Types.Media;
using Trackster.Api.Features.Movies.Types;
using Trackster.Api.Features.Shows.Types;

namespace Trackster.Api.Features.Media;

public static class MediaMapper
{
    public static MediaResult FromMovie(WatchedMovie watchedMovie)
    {
        return new MediaResult
        {
            Identifier = watchedMovie.Movie.Identifier,
            Title = watchedMovie.Movie.Title,
            Slug = watchedMovie.Movie.Slug,
            Year = watchedMovie.Movie.Year,
            Overview = watchedMovie.Movie.Overview,
            Poster = watchedMovie.Movie.Poster,
            TMDB = watchedMovie.Movie.TMDB,
            Type = MediaType.Movie.ToString(),
            WatchedAt = watchedMovie.WatchedAt,
        };
    }

    public static MediaResult FromEpisode(WatchedEpisode watchedEpisode)
    {
        return new MediaResult
        {
            Identifier = watchedEpisode.Episode.Season.Show.Identifier,
            Title = watchedEpisode.Episode.Title,
            ParentTitle = watchedEpisode.Episode.Season.Title,
            GrandParentTitle = watchedEpisode.Episode.Season.Show.Title,
            Slug = watchedEpisode.Episode.Season.Show.Slug,
            Year = watchedEpisode.Episode.Season.Show.Year,
            Overview = watchedEpisode.Episode.Season.Show.Overview,
            Poster = watchedEpisode.Episode.Season.Show.Poster,
            TMDB = watchedEpisode.Episode.Season.Show.TMDB,
            Type = MediaType.Episode.ToString(),
            SeasonNumber = watchedEpisode.Episode.Season.Number,
            EpisodeNumber = watchedEpisode.Episode.Number,
            WatchedAt = watchedEpisode.WatchedAt,
        };
    }

    public static List<MediaResult> Aggregate(IEnumerable<WatchedMovie> movies, IEnumerable<WatchedEpisode> episodes, int limit)
    {
        return movies
            .Select(FromMovie)
            .Concat(episodes.Select(FromEpisode))
            .OrderByDescending(media => media.WatchedAt)
            .Take(limit)
            .ToList();
    }
}

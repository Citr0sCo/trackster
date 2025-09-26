using Trackster.Api.Data.Records;
using Trackster.Api.Features.Media.Types;
using Trackster.Api.Features.Shows.Types;

namespace Trackster.Api.Features.Shows;

public static class ShowMapper
{

    public static Show Map(ShowRecord show, List<GenreRecord> genres)
    {
        return new Show
        {
            Identifier = show.Identifier,
            Title = show.Title,
            Slug = show.Slug,
            Year = show.Year,
            TMDB = show.TMDB,
            Poster = show.Poster,
            Overview = show.Overview,
            Genres = genres.ConvertAll(x => new Genre
            {
                Identifier = x.Identifier,
                Name = x.Name
            })
        };
    }

    public static Season MapSeason(SeasonRecord season, List<GenreRecord> genres)
    {
        return new Season
        {
            Identifier = season.Identifier,
            Title = season.Title,
            Number = season.Number,
            Show = Map(season.Show, genres)
        };
    }
    
    public static Episode MapEpisode(EpisodeRecord episode, List<GenreRecord> genres)
    {
        return new Episode
        {
            Identifier = episode.Identifier,
            Title = episode.Title,
            Number = episode.Number,
            Season = MapSeason(episode.Season, genres),
        };
    }
}
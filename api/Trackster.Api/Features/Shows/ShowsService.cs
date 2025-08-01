using Trackster.Api.Core.Helpers;
using Trackster.Api.Data.Records;
using Trackster.Api.Features.Media.Importers.TmdbImporter;
using Trackster.Api.Features.Media.Importers.TraktImporter.Types;
using Trackster.Api.Features.Shows.Types;

namespace Trackster.Api.Features.Shows;

public interface IShowsService
{
    GetAllShowsResponse GetAllWatchedShows(string username);
    Task<EpisodeRecord> SearchForEpisode(string showTitle, string episodeTitle, int year, int seasonNumber);
    GetShowResponse GetShowBySlug(string slug);
    Task ImportShows(string username, List<TraktShowResponse> movies);
    void ImportEpisode(string username, ShowRecord show, SeasonRecord season, EpisodeRecord episode);
}

public class ShowsService : IShowsService
{
    private readonly IMediaRepository _repository;
    private readonly TmdbImportProvider _detailsProvider;

    public ShowsService(ShowsRepository repository)
    {
        _repository = repository;
        _detailsProvider = new TmdbImportProvider();
    }
    
    public GetAllShowsResponse GetAllWatchedShows(string username)
    {
        var shows = _repository.GetAllWatchedShows(username);

        return new GetAllShowsResponse
        {
            WatchedShows = shows
        };
    }
    
    public async Task<EpisodeRecord> SearchForEpisode(string showTitle, string episodeTitle, int year, int seasonNumber)
    {
        var searchResults = await _detailsProvider.FindShowByTitleAndYear(showTitle, year);
        var tmdbReference = searchResults.Results.FirstOrDefault()?.Id.ToString();
        var parsedShow = await _detailsProvider.GetDetailsForShow(tmdbReference ?? "");
        var parsedSeason = await _detailsProvider.GetDetailsForSeason(parsedShow.Identifier, seasonNumber);
        var parsedEpisode = parsedSeason.Episodes.FirstOrDefault(x => x.Title.ToLower() == episodeTitle.ToLower());
        
        var show = new ShowRecord
        {
            Identifier = Guid.NewGuid(),
            Title = parsedShow.Title,
            Slug = SlugHelper.GenerateSlugFor(parsedShow.Title),
            Overview = parsedShow.Overview,
            Poster = $"https://image.tmdb.org/t/p/w185{parsedShow.PosterUrl}",
            TMDB = parsedShow.Identifier.ToString(),
            Year = parsedShow.FirstAirDate.Year
        };

        var season = new SeasonRecord
        {
            Identifier = Guid.NewGuid(),
            Show = show,
            Number = seasonNumber,
            Title = parsedSeason.Title
        };

        return new EpisodeRecord
        {
            Identifier = Guid.NewGuid(),
            Season = season,
            Number = parsedEpisode.EpisodeNumber,
            Title = parsedEpisode.Title,
        };
    }

    public GetShowResponse GetShowBySlug(string slug)
    {
        var show = _repository.GetShowBySlug(slug);

        if (show != null)
        {
            return new GetShowResponse
            {
                Show = new Show
                {
                    Identifier = show.Identifier,
                    Slug = show.Slug,
                    Title = show.Title,
                    Year = show.Year,
                    Overview = show.Overview,
                    Poster = show.Poster,
                    TMDB = show.TMDB
                }
            };
        }
        
        return new GetShowResponse();
    }

    public Task ImportShows(string username, List<TraktShowResponse> shows)
    {
        return _repository.ImportShows(username, shows);
    }

    public void ImportEpisode(string username, ShowRecord show, SeasonRecord season, EpisodeRecord episode)
    {
        _repository.ImportEpisode(username, show, season, episode);
    }

    public GetSeasonResponse GetSeasonByNumber(string slug, int seasonNumber)
    {
        var season = _repository.GetSeasonByNumber(slug, seasonNumber);

        if (season != null)
        {
            return new GetSeasonResponse
            {
                Season = new Season
                {
                    Identifier = season.Identifier,
                    Title = season.Title,
                    Number = season.Number
                }
            };
        }
        
        return new GetSeasonResponse();
    }

    public GetEpisodeResponse GetEpisodeByNumber(string slug, int seasonNumber, int episodeNumber)
    {
        var episode = _repository.GetEpisodeByNumber(slug, seasonNumber, episodeNumber);

        if (episode != null)
        {
            return new GetEpisodeResponse
            {
                Episode = new Episode
                {
                    Identifier = episode.Identifier,
                    Title = episode.Title,
                    Number = episode.Number
                }
            };
        }
        
        return new GetEpisodeResponse();
    }

    public GetEpisodeWatchedHistoryResponse GetWatchedHistoryByEpisodeNumber(string username, string slug, int seasonNumber, int episodeNumber)
    {
        var episodeWatchHistory = _repository.GetWatchedHistoryByEpisodeNumber(username, slug, seasonNumber, episodeNumber);

        if (episodeWatchHistory != null)
        {
            return new GetEpisodeWatchedHistoryResponse
            {
                WatchedEpisodes = episodeWatchHistory.ConvertAll((episode) =>
                {
                    return new WatchedEpisode
                    {
                        WatchedAt = episode.WatchedAt
                    };
                })
            };
        }
        
        return new GetEpisodeWatchedHistoryResponse();
    }
}
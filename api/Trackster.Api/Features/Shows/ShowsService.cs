using Newtonsoft.Json;
using Trackster.Api.Core.Helpers;
using Trackster.Api.Data.Records;
using Trackster.Api.Features.Media.Importers.TmdbImporter;
using Trackster.Api.Features.Shows.Types;

namespace Trackster.Api.Features.Shows;

public interface IShowsService
{
    GetAllShowsResponse GetAllWatchedShows(string username, int results, int page);
    Task<EpisodeRecord> SearchForEpisode(string showTitle, string episodeTitle, int year, int seasonNumber);
    GetShowResponse GetShowBySlug(string slug);
    Task<ShowRecord?> GetShowByTmdbId(string tmdbId);
    Task<SeasonRecord?> GetSeasonBy(int seasonNumber, Guid showIdentifier);
    Task<EpisodeRecord?> GetEpisodeBy(int episodeNumber, Guid seasonIdentifier);
    Task ImportShow(UserRecord user, ShowRecord show);
    Task ImportSeason(UserRecord user, ShowRecord show, SeasonRecord season);
    Task ImportEpisode(UserRecord user, ShowRecord show, SeasonRecord season, EpisodeRecord episode);
    GetSeasonResponse GetSeasonByNumber(string slug, int seasonNumber);
    GetEpisodeResponse GetEpisodeByNumber(string slug, int seasonNumber, int episodeNumber);
    GetEpisodeWatchedHistoryResponse GetWatchedHistoryByEpisodeNumber(string username, string slug, int seasonNumber, int episodeNumber);
    Task MarkEpisodeAsWatched(string username, string showTmdbId, int seasonNumber, int episodeNumber, DateTime watchedAt);
}

public class ShowsService : IShowsService
{
    private readonly IShowsRepository _repository;
    private readonly TmdbImportProvider _detailsProvider;

    public ShowsService(ShowsRepository repository)
    {
        _repository = repository;
        _detailsProvider = new TmdbImportProvider();
    }
    
    public GetAllShowsResponse GetAllWatchedShows(string username, int results, int page)
    {
        var shows = _repository.GetAllWatchedShows(username, results, page);

        return new GetAllShowsResponse
        {
            WatchedShows = shows
        };
    }
    
    public async Task<EpisodeRecord> SearchForEpisode(string showTitle, string episodeTitle, int year, int seasonNumber)
    {
        var searchResults = await _detailsProvider.FindShowByTitleAndYear(showTitle, year);

        if (searchResults.Results.Count == 0)
        {
            Console.WriteLine($"[ERROR] - Failed to find show by title ({showTitle}) and year ({year}).");
            Console.WriteLine(JsonConvert.SerializeObject(searchResults));
            return new EpisodeRecord();
        }
        
        var tmdbReference = searchResults.Results.First().Id.ToString();
        
        if (tmdbReference.Length == 0)
        {
            Console.WriteLine($"[ERROR] - Failed to find show identifier by title ({showTitle}) and year ({year}).");
            Console.WriteLine(JsonConvert.SerializeObject(searchResults));
            return new EpisodeRecord();
        }
        
        var parsedShow = await _detailsProvider.GetDetailsForShow(tmdbReference!);
        
        if (parsedShow.Identifier == 0)
        {
            Console.WriteLine($"[ERROR] - Failed to find show details by tmdb reference ({tmdbReference}), title ({showTitle}) and year ({year}).");
            Console.WriteLine(JsonConvert.SerializeObject(searchResults));
            return new EpisodeRecord();
        }
        
        var parsedSeason = await _detailsProvider.GetDetailsForSeason(parsedShow.Identifier, seasonNumber);
        
        if (parsedSeason.Episodes.Count == 0)
        {
            Console.WriteLine($"[ERROR] - Failed to find season by tmdb reference ({parsedShow.Identifier}), season number ({seasonNumber}), title ({showTitle}) and year ({year}).");
            Console.WriteLine(JsonConvert.SerializeObject(searchResults));
            return new EpisodeRecord();
        }
        
        var parsedEpisode = parsedSeason.Episodes.FirstOrDefault(x => x.Title.ToLower() == episodeTitle.ToLower());
        
        if (parsedEpisode.Id == 0)
        {
            Console.WriteLine($"[ERROR] - Failed to find episode by title ({episodeTitle}), tmdb reference ({parsedShow.Identifier}), season number ({seasonNumber}), title ({showTitle}) and year ({year}).");
            Console.WriteLine(JsonConvert.SerializeObject(searchResults));
            return new EpisodeRecord();
        }
        
        var show = new ShowRecord
        {
            Identifier = Guid.NewGuid(),
            Title = parsedShow.Title,
            Slug = SlugHelper.GenerateSlugFor(parsedShow.Title),
            Overview = parsedShow.Overview,
            Poster = $"https://image.tmdb.org/t/p/w300{parsedShow.PosterUrl}",
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

    public async Task<ShowRecord?> GetShowByTmdbId(string tmdbId)
    {
        return await _repository.GetShowByTmdbId(tmdbId);    
    }
    
    public async Task<SeasonRecord?> GetSeasonBy(int seasonNumber, Guid showIdentifier)
    {
        return await _repository.GetSeasonBy(seasonNumber, showIdentifier);
    }
    
    public async Task<EpisodeRecord?> GetEpisodeBy(int episodeNumber, Guid seasonIdentifier)
    {
        return await _repository.GetEpisodeBy(episodeNumber, seasonIdentifier);
    }

    public async Task ImportShow(UserRecord user, ShowRecord show)
    {
        await _repository.ImportShow(user, show);
    }

    public async Task ImportSeason(UserRecord user, ShowRecord show, SeasonRecord season)
    {
        await _repository.ImportSeason(user, show, season);
    }

    public async Task ImportEpisode(UserRecord user, ShowRecord show, SeasonRecord season, EpisodeRecord episode)
    {
        await _repository.ImportEpisode(user, show, season, episode);
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

    public async Task MarkEpisodeAsWatched(string username, string showTmdbId, int seasonNumber, int episodeNumber, DateTime watchedAt)
    {
        await _repository.MarkEpisodeAsWatched(username, showTmdbId, seasonNumber, episodeNumber, watchedAt);
    }
}
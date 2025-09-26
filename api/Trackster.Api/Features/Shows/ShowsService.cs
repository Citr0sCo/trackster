using Newtonsoft.Json;
using Trackster.Api.Core.Helpers;
using Trackster.Api.Data.Records;
using Trackster.Api.Features.Media.Importers.TmdbImporter;
using Trackster.Api.Features.Shows.Types;

namespace Trackster.Api.Features.Shows;

public interface IShowsService
{
    GetAllWatchedEpisodesResponse GetAllWatchedEpisodes(string username, int results, int page);
    Task<EpisodeRecord> SearchForEpisode(string showTitle, string seasonTitle, string episodeTitle, int year, int seasonNumber, bool requestDebug);
    GetShowResponse GetShowBySlug(string slug);
    Task<ShowRecord?> GetShowByTmdbId(string tmdbId);
    Task<SeasonRecord?> GetSeasonBy(int seasonNumber, Guid showIdentifier);
    Task<EpisodeRecord?> GetEpisodeBy(int episodeNumber, Guid seasonIdentifier);
    Task ImportShow(UserRecord user, ShowRecord show, List<GenreRecord> genres);
    Task ImportSeason(UserRecord user, ShowRecord show, SeasonRecord season);
    Task ImportEpisode(UserRecord user, ShowRecord show, SeasonRecord season, EpisodeRecord episode);
    GetSeasonResponse GetSeasonByNumber(string slug, int seasonNumber);
    GetEpisodeResponse GetEpisodeByNumber(string slug, int seasonNumber, int episodeNumber);
    GetEpisodeWatchedHistoryResponse GetWatchedHistoryByEpisodeNumber(string username, string slug, int seasonNumber, int episodeNumber);
    Task MarkEpisodeAsWatched(UserRecord user, ShowRecord show, SeasonRecord season, EpisodeRecord episode, DateTime watchedAt);
    EpisodeUserRecord? GetWatchedShowByLastWatchedAt(string username, string idsTmdb, DateTime watchedAt);
    ShowRecord? GetShowByReference(Guid identifier);
    SeasonRecord? GetSeasonByReference(Guid identifier);
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
    
    public GetAllWatchedEpisodesResponse GetAllWatchedEpisodes(string username, int results, int page)
    {
        var episodes = _repository.GetAllWatchedEpisodes(username, results, page);

        return new GetAllWatchedEpisodesResponse
        {
            WatchedEpisodes = episodes
        };
    }
    
    public async Task<EpisodeRecord> SearchForEpisode(string showTitle, string seasonTitle, string episodeTitle, int year, int seasonNumber, bool requestDebug)
    {
        var searchResults = await _detailsProvider.FindShowByTitleAndYear(showTitle, year, requestDebug);

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
        
        var parsedShow = await _detailsProvider.GetDetailsForShow(tmdbReference!, requestDebug);
        
        if (parsedShow.Identifier == 0)
        {
            Console.WriteLine($"[ERROR] - Failed to find show details by tmdb reference ({tmdbReference}), title ({showTitle}) and year ({year}).");
            Console.WriteLine(JsonConvert.SerializeObject(searchResults));
            return new EpisodeRecord();
        }
        
        if(seasonNumber > 2000 && int.TryParse(seasonTitle.Replace("Season ", ""), out var seasonNumberParsed))
            seasonNumber = seasonNumberParsed;
        
        var parsedSeason = await _detailsProvider.GetDetailsForSeason(parsedShow.Identifier, seasonNumber, requestDebug);
        
        if (parsedSeason.Episodes.Count == 0)
        {
            Console.WriteLine($"[ERROR] - Failed to find season by tmdb reference ({parsedShow.Identifier}), season number ({seasonNumber}), title ({showTitle}) and year ({year}).");
            Console.WriteLine(JsonConvert.SerializeObject(searchResults));
            return new EpisodeRecord();
        }
        
        var parsedEpisode = parsedSeason.Episodes.FirstOrDefault(x => x.Title.ToLower() == episodeTitle.ToLower());
        
        if (parsedEpisode?.Id == 0)
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

        var episode = new EpisodeRecord
        {
            Identifier = Guid.NewGuid(),
            Season = season,
            Number = parsedEpisode?.EpisodeNumber ?? 0,
            Title = parsedEpisode?.Title ?? "",
        };

        await _repository.SaveEpisode(show, season, episode);
        
        return episode;
    }

    public GetShowResponse GetShowBySlug(string slug)
    {
        var show = _repository.GetShowBySlug(slug);

        if (show != null)
        {
            return new GetShowResponse
            {
                Show = show
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

    public async Task ImportShow(UserRecord user, ShowRecord show, List<GenreRecord> genres)
    {
        await _repository.ImportShow(user, show, genres);
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
                Season = season
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
                Episode = episode
            };
        }
        
        return new GetEpisodeResponse();
    }

    public async Task<GetEpisodeResponse> ImportDataForEpisode(string slug, int seasonNumber, int episodeNumber, bool requestDebug = false)
    {
        var episode = _repository.GetEpisodeByNumber(slug, seasonNumber, episodeNumber);

        if (episode != null)
        {
            var details = await _detailsProvider.GetEpisodeDetails(episode.Season.Show.TMDB, seasonNumber, episodeNumber, requestDebug);

            var showDetails = await _detailsProvider.GetDetailsForShow(episode.Season.Show.TMDB, requestDebug);

            var genres = await _repository.FindOrCreateGenres(showDetails.Genres.ConvertAll((genre) => genre.Name));

            var updatedEpisode = await _repository.UpdateEpisode(new EpisodeRecord
            {
                Identifier = episode.Identifier,
                Title = details.Title ?? episode.Season.Show.Title,
            }, genres);

            return new GetEpisodeResponse
            {
                Episode = ShowMapper.MapEpisode(updatedEpisode, genres)
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

    public async Task MarkEpisodeAsWatched(UserRecord user, ShowRecord show, SeasonRecord season, EpisodeRecord episode, DateTime watchedAt)
    {
        await _repository.MarkEpisodeAsWatched(user, show, season, episode, watchedAt);
    }

    public EpisodeUserRecord? GetWatchedShowByLastWatchedAt(string username, string idsTmdb, DateTime watchedAt)
    {
        return _repository.GetWatchedShowByLastWatchedAt(username, idsTmdb, watchedAt);
    }

    public ShowRecord? GetShowByReference(Guid identifier)
    {
        return _repository.GetShowByReference(identifier);
    }

    public SeasonRecord? GetSeasonByReference(Guid identifier)
    {
        return _repository.GetSeasonByReference(identifier);
    }
}
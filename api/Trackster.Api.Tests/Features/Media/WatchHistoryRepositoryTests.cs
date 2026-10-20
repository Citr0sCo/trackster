using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Trackster.Api.Data;
using Trackster.Api.Data.Records;
using Trackster.Api.Features.Movies;
using Trackster.Api.Features.Shows;

namespace HomeBoxLanding.Api.Tests.Features.Media;

[TestFixture]
[NonParallelizable]
public class WatchHistoryRepositoryTests
{
    private static readonly DateTime FirstWatch = new(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc);

    private string _databasePath = string.Empty;

    [SetUp]
    public void SetUp()
    {
        _databasePath = Path.Combine(Path.GetTempPath(), $"trackster-tests-{Guid.NewGuid():N}.db");
        Environment.SetEnvironmentVariable("TRACKSTER_DATABASE_PATH", _databasePath);

        using var context = new DatabaseContext();
        context.Database.EnsureCreated();
    }

    [TearDown]
    public void TearDown()
    {
        Environment.SetEnvironmentVariable("TRACKSTER_DATABASE_PATH", null);
        SqliteConnection.ClearAllPools();

        if (File.Exists(_databasePath))
            File.Delete(_databasePath);
    }

    [Test]
    public async Task MarkMovieAsWatched_IsIdempotentForTheSameWatch()
    {
        var repository = new MoviesRepository();
        var user = CreateUser();
        var movie = CreateMovie();

        var firstImport = await repository.MarkMovieAsWatched(user, movie, FirstWatch);
        var secondImport = await repository.MarkMovieAsWatched(user, CreateMovie(), FirstWatch.AddSeconds(20));
        var thirdImport = await repository.MarkMovieAsWatched(user, CreateMovie(), FirstWatch);

        Assert.That(firstImport, Is.True);
        Assert.That(secondImport, Is.False);
        Assert.That(thirdImport, Is.False);

        using var context = new DatabaseContext();
        Assert.That(context.MovieUserLinks.Count(), Is.EqualTo(1));
        Assert.That(context.Movies.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task MarkMovieAsWatched_KeepsDistinctRewatches()
    {
        var repository = new MoviesRepository();
        var user = CreateUser();
        var movie = CreateMovie();

        await repository.MarkMovieAsWatched(user, movie, FirstWatch);
        await repository.MarkMovieAsWatched(user, movie, FirstWatch.AddDays(30));

        using var context = new DatabaseContext();
        Assert.That(context.MovieUserLinks.Count(), Is.EqualTo(2));
        Assert.That(context.Movies.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task MarkMovieAsWatched_FillsInMissingWatchRecordsWhenSyncingAgain()
    {
        var repository = new MoviesRepository();
        var user = CreateUser();
        var movie = CreateMovie();

        // Simulates an import that only stored the first watch.
        await repository.MarkMovieAsWatched(user, movie, FirstWatch);

        // A later sync replays the whole Trakt history and should add only the missing watches.
        var imported = 0;

        foreach (var watchedAt in new[] { FirstWatch, FirstWatch.AddDays(1), FirstWatch.AddDays(2) })
        {
            if (await repository.MarkMovieAsWatched(user, movie, watchedAt))
                imported++;
        }

        Assert.That(imported, Is.EqualTo(2));

        using var context = new DatabaseContext();
        Assert.That(context.MovieUserLinks.Count(), Is.EqualTo(3));
        Assert.That(context.Movies.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task MarkEpisodeAsWatched_IsIdempotentAndDoesNotDuplicateMedia()
    {
        var repository = new ShowsRepository();
        var user = CreateUser();
        var (show, season, episode) = CreateEpisode();
        var (duplicateShow, duplicateSeason, duplicateEpisode) = CreateEpisode();

        var firstImport = await repository.MarkEpisodeAsWatched(user, show, season, episode, FirstWatch);
        var secondImport = await repository.MarkEpisodeAsWatched(user, duplicateShow, duplicateSeason, duplicateEpisode, FirstWatch.AddSeconds(30));

        Assert.That(firstImport, Is.True);
        Assert.That(secondImport, Is.False);

        using var context = new DatabaseContext();
        Assert.That(context.EpisodeUserLinks.Count(), Is.EqualTo(1));
        Assert.That(context.Shows.Count(), Is.EqualTo(1));
        Assert.That(context.Seasons.Count(), Is.EqualTo(1));
        Assert.That(context.Episodes.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task MarkEpisodeAsWatched_KeepsDistinctRewatches()
    {
        var repository = new ShowsRepository();
        var user = CreateUser();
        var (show, season, episode) = CreateEpisode();

        await repository.MarkEpisodeAsWatched(user, show, season, episode, FirstWatch);
        await repository.MarkEpisodeAsWatched(user, show, season, episode, FirstWatch.AddDays(7));

        using var context = new DatabaseContext();
        Assert.That(context.EpisodeUserLinks.Count(), Is.EqualTo(2));
        Assert.That(context.Episodes.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task SearchWatchedMovies_FindsWatchedTitlesOnly()
    {
        var repository = new MoviesRepository();
        var user = CreateUser();

        await repository.MarkMovieAsWatched(user, CreateMovie("603", "The Matrix", "the-matrix"), FirstWatch);
        await repository.MarkMovieAsWatched(user, CreateMovie("693134", "Dune: Part Two", "dune-part-two"), FirstWatch.AddDays(1));

        var results = repository.SearchWatchedMovies(user.Username, "matr", 20);

        Assert.That(results, Has.Count.EqualTo(1));
        Assert.That(results[0].Movie.Title, Is.EqualTo("The Matrix"));
        Assert.That(results[0].Movie.Slug, Is.EqualTo("the-matrix"));
        Assert.That(results[0].WatchedAt, Is.EqualTo(FirstWatch));
    }

    [Test]
    public async Task SearchWatchedEpisodes_ReturnsEachMatchingEpisodeOnceWithItsLatestWatch()
    {
        var repository = new ShowsRepository();
        var user = CreateUser();
        var show = CreateShow("1396", "Breaking Bad", "breaking-bad");
        var season = CreateSeason(show);

        await repository.MarkEpisodeAsWatched(user, show, season, CreateEpisodeRecord(1, "Pilot", season), FirstWatch);
        await repository.MarkEpisodeAsWatched(user, show, season, CreateEpisodeRecord(2, "Cat's in the Bag...", season), FirstWatch.AddDays(1));
        await repository.MarkEpisodeAsWatched(user, show, season, CreateEpisodeRecord(2, "Cat's in the Bag...", season), FirstWatch.AddDays(8));

        var results = repository.SearchWatchedEpisodes(user.Username, "breaking", 20);

        Assert.That(results, Has.Count.EqualTo(2));
        Assert.That(results[0].Episode.Season.Show.Title, Is.EqualTo("Breaking Bad"));
        Assert.That(results[0].Episode.Number, Is.EqualTo(2));
        Assert.That(results[0].WatchedAt, Is.EqualTo(FirstWatch.AddDays(8)));
        Assert.That(results[1].Episode.Number, Is.EqualTo(1));
    }

    [Test]
    public async Task RemoveFromWatchHistory_RemovesOnlyTheUsersWatchRecords()
    {
        var repository = new MoviesRepository();
        var user = CreateUser();
        var movie = CreateMovie();

        await repository.MarkMovieAsWatched(user, movie, FirstWatch);
        await repository.MarkMovieAsWatched(user, movie, FirstWatch.AddDays(3));

        var removed = await repository.RemoveFromWatchHistory(user.Username, movie.Slug);

        Assert.That(removed, Is.EqualTo(2));
        Assert.That(await repository.RemoveFromWatchHistory(user.Username, movie.Slug), Is.EqualTo(0));
        Assert.That(repository.SearchWatchedMovies(user.Username, movie.Title, 20), Is.Empty);
        Assert.That(repository.GetMovieByTmdbId(movie.TMDB), Is.Not.Null);
    }

    [Test]
    public async Task RemoveEpisodeFromWatchHistory_RemovesOnlyThatEpisode()
    {
        var repository = new ShowsRepository();
        var user = CreateUser();
        var (show, season, episode) = CreateEpisode();
        var secondEpisode = CreateEpisodeRecord(2, "Second", season);

        await repository.MarkEpisodeAsWatched(user, show, season, episode, FirstWatch);
        await repository.MarkEpisodeAsWatched(user, show, season, secondEpisode, FirstWatch.AddDays(1));

        var removed = await repository.RemoveFromWatchHistory(user.Username, show.Slug, season.Number, episode.Number);

        Assert.That(removed, Is.EqualTo(1));
        Assert.That(repository.GetEpisodeBy(episode.Number, season.Identifier), Is.Not.Null);
        Assert.That(repository.GetEpisodeBy(secondEpisode.Number, season.Identifier), Is.Not.Null);

        var results = repository.SearchWatchedEpisodes(user.Username, show.Title, 20);

        Assert.That(results, Has.Count.EqualTo(1));
        Assert.That(results[0].Episode.Number, Is.EqualTo(2));
    }

    private static UserRecord CreateUser(string username = "alice")
    {
        return new UserRecord
        {
            Identifier = Guid.NewGuid(),
            Username = username,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    private static MovieRecord CreateMovie(string tmdb = "438631", string title = "Dune", string slug = "dune")
    {
        return new MovieRecord
        {
            Identifier = Guid.NewGuid(),
            Title = title,
            Slug = slug,
            Year = 2021,
            TMDB = tmdb,
            Overview = "Overview",
            Poster = "https://example.test/poster.jpg"
        };
    }

    private static ShowRecord CreateShow(string tmdb = "1396", string title = "Breaking Bad", string slug = "breaking-bad")
    {
        return new ShowRecord
        {
            Identifier = Guid.NewGuid(),
            Title = title,
            Slug = slug,
            Year = 2008,
            TMDB = tmdb,
            Overview = "Overview",
            Poster = "https://example.test/poster.jpg"
        };
    }

    private static SeasonRecord CreateSeason(ShowRecord show, int number = 1)
    {
        return new SeasonRecord
        {
            Identifier = Guid.NewGuid(),
            Number = number,
            Title = $"Season {number}",
            Show = show
        };
    }

    private static EpisodeRecord CreateEpisodeRecord(int number, string title, SeasonRecord season)
    {
        return new EpisodeRecord
        {
            Identifier = Guid.NewGuid(),
            Number = number,
            Title = title,
            Season = season
        };
    }

    private static (ShowRecord Show, SeasonRecord Season, EpisodeRecord Episode) CreateEpisode()
    {
        var show = CreateShow();
        var season = CreateSeason(show);
        var episode = CreateEpisodeRecord(1, "Pilot", season);

        return (show, season, episode);
    }
}

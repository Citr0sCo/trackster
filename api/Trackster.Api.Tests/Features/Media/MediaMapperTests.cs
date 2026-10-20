using NUnit.Framework;
using Trackster.Api.Features.Media;
using Trackster.Api.Features.Media.Types;
using Trackster.Api.Features.Movies.Types;
using Trackster.Api.Features.Shows.Types;

namespace HomeBoxLanding.Api.Tests.Features.Media;

[TestFixture]
public class MediaMapperTests
{
    private static readonly DateTime Now = new(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc);

    [Test]
    public void Aggregate_OrdersMoviesAndEpisodesByMostRecentWatch()
    {
        var movie = new WatchedMovie
        {
            WatchedAt = Now.AddDays(-3),
            Movie = new Movie { Identifier = Guid.NewGuid(), Title = "Older movie", Slug = "older-movie", TMDB = "1" }
        };
        var episode = new WatchedEpisode
        {
            WatchedAt = Now,
            Episode = new Episode
            {
                Identifier = Guid.NewGuid(),
                Title = "Newest episode",
                Number = 2,
                Season = new Season
                {
                    Identifier = Guid.NewGuid(),
                    Title = "Season 1",
                    Number = 1,
                    Show = new Show { Identifier = Guid.NewGuid(), Title = "Show", Slug = "show", TMDB = "2" }
                }
            }
        };

        var media = MediaMapper.Aggregate(new[] { movie }, new[] { episode }, 10);

        Assert.That(media, Has.Count.EqualTo(2));
        Assert.That(media[0].Title, Is.EqualTo("Newest episode"));
        Assert.That(media[0].Type, Is.EqualTo(MediaType.Episode.ToString()));
        Assert.That(media[0].Slug, Is.EqualTo("show"));
        Assert.That(media[0].SeasonNumber, Is.EqualTo(1));
        Assert.That(media[1].Title, Is.EqualTo("Older movie"));
        Assert.That(media[1].Type, Is.EqualTo(MediaType.Movie.ToString()));
    }

    [Test]
    public void Aggregate_RespectsTheResultLimit()
    {
        var movies = Enumerable.Range(0, 5)
            .Select(index => new WatchedMovie
            {
                WatchedAt = Now.AddMinutes(index),
                Movie = new Movie { Identifier = Guid.NewGuid(), Title = $"Movie {index}", Slug = $"movie-{index}", TMDB = index.ToString() }
            })
            .ToList();

        var media = MediaMapper.Aggregate(movies, [], 3);

        Assert.That(media, Has.Count.EqualTo(3));
        Assert.That(media[0].Title, Is.EqualTo("Movie 4"));
    }
}

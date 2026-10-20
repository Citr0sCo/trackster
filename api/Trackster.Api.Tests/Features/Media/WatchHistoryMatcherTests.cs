using NUnit.Framework;
using Trackster.Api.Features.Media;

namespace HomeBoxLanding.Api.Tests.Features.Media;

[TestFixture]
public class WatchHistoryMatcherTests
{
    private static readonly DateTime WatchedAt = new(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc);

    [TestCase(0)]
    [TestCase(15)]
    [TestCase(59)]
    public void IsSameWatch_TreatsTimestampsWithinOneMinuteAsTheSameWatch(int seconds)
    {
        Assert.That(WatchHistoryMatcher.IsSameWatch(WatchedAt, WatchedAt.AddSeconds(seconds)), Is.True);
        Assert.That(WatchHistoryMatcher.IsSameWatch(WatchedAt, WatchedAt.AddSeconds(-seconds)), Is.True);
    }

    [TestCase(61)]
    [TestCase(3600)]
    [TestCase(24 * 3600)]
    public void IsSameWatch_TreatsDistinctWatchesAsDifferent(int seconds)
    {
        Assert.That(WatchHistoryMatcher.IsSameWatch(WatchedAt, WatchedAt.AddSeconds(seconds)), Is.False);
    }

    [Test]
    public void Contains_FindsExistingWatchRegardlessOfTicks()
    {
        var existing = new[] { WatchedAt.AddDays(-2), WatchedAt.AddTicks(-1) };

        Assert.That(WatchHistoryMatcher.Contains(existing, WatchedAt), Is.True);
        Assert.That(WatchHistoryMatcher.Contains(existing, WatchedAt.AddHours(2)), Is.False);
    }

    [Test]
    public void Distinct_CollapsesNearDuplicateTimestamps()
    {
        var result = WatchHistoryMatcher.Distinct(new[]
        {
            WatchedAt,
            WatchedAt.AddSeconds(30),
            WatchedAt.AddDays(1)
        });

        Assert.That(result, Has.Count.EqualTo(2));
    }
}

namespace Trackster.Api.Features.Media;

public static class WatchHistoryMatcher
{
    /// <summary>
    /// Trakt timestamps are stable to the second, but records created by webhooks or manual
    /// marking can drift by a few seconds from the Trakt equivalent. A one minute window keeps
    /// a sync idempotent without merging genuine re-watches.
    /// </summary>
    private static readonly TimeSpan Tolerance = TimeSpan.FromMinutes(1);

    public static bool IsSameWatch(DateTime existing, DateTime incoming)
    {
        return (existing - incoming).Duration() <= Tolerance;
    }

    public static bool Contains(IEnumerable<DateTime> existingWatches, DateTime incoming)
    {
        return existingWatches.Any(existing => IsSameWatch(existing, incoming));
    }

    public static List<DateTime> Distinct(IEnumerable<DateTime> watchedAt)
    {
        var distinct = new List<DateTime>();

        foreach (var entry in watchedAt)
        {
            if (!Contains(distinct, entry))
                distinct.Add(entry);
        }

        return distinct;
    }
}

using System.ComponentModel.DataAnnotations;

namespace Trackster.Api.Data.Records;

public class EpisodeUserRecord
{
    [Key]
    public Guid Identifier { get; set; }
    public UserRecord User { get; set; }
    public EpisodeRecord Episode { get; set; }
    public DateTime WatchedAt { get; set; }
}
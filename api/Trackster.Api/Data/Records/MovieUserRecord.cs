using System.ComponentModel.DataAnnotations;

namespace Trackster.Api.Data.Records;

public class MovieUserRecord
{
    [Key]
    public Guid Identifier { get; set; }

    public UserRecord User { get; set; }
    public MovieRecord Movie { get; set; }
    public DateTime WatchedAt { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace Trackster.Api.Data.Records;

public class SessionRecord
{
    [Key]
    public Guid Identifier { get; set; }

    public DateTime TimeToLive { get; set; }
    public UserRecord User { get; set; }
}
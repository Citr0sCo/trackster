using System.ComponentModel.DataAnnotations;
using Trackster.Api.Data.Records;

namespace Trackster.Api.Features.Sessions.Types;

public class SessionRecord
{
    [Key]
    public Guid Identifier { get; set; }

    public DateTime TimeToLive { get; set; }
    public UserRecord User { get; set; }
}
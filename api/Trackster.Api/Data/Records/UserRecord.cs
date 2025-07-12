using System.ComponentModel.DataAnnotations;

namespace Trackster.Api.Data.Records;

public class UserRecord
{
    [Key]
    public Guid Identifier { get; set; }
    public string Username { get; set; }
}
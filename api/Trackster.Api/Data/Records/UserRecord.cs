using System.ComponentModel.DataAnnotations;

namespace Trackster.Api.Data.Records;

public class UserRecord
{
    [Key]
    public Guid Identifier { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
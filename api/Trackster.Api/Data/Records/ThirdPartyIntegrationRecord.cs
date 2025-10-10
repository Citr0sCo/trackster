using System.ComponentModel.DataAnnotations;
using Trackster.Api.Features.Auth.Types;

namespace Trackster.Api.Data.Records;

public class ThirdPartyIntegrationRecord
{
    [Key]
    public Guid Identifier { get; set; }

    public Provider Provider { get; set; }
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public DateTime ExpiresAt { get; set; }
}
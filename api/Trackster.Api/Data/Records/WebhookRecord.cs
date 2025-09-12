using System.ComponentModel.DataAnnotations;

namespace Trackster.Api.Data.Records;

public class WebhookRecord
{
    [Key]
    public Guid Identifier { get; set; }

    public string ApiKey { get; set; }
    public WebhookProvider Provider { get; set; }
    public UserRecord User { get; set; }
}

public enum WebhookProvider
{
    Unknown,
    Plex
}
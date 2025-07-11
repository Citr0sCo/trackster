namespace Trackster.Api.Features.Webhook.Types;

public class PlexWebhookRequest
{
    public string @event { get; set; }
    public bool user { get; set; }
    public bool owner { get; set; }
    public PlexAccount Account { get; set; }
    public PlexServer Server { get; set; }
    public PlexPlayer Player { get; set; }
    public PlexMetadata Metadata { get; set; }

    public class PlexAccount
    {
        public int id { get; set; }
        public string thumb { get; set; }
        public string title { get; set; }
    }

    public class PlexMetadata
    {
        public string librarySectionType { get; set; }
        public string ratingKey { get; set; }
        public string key { get; set; }
        public string parentRatingKey { get; set; }
        public string grandparentRatingKey { get; set; }
        public string guid { get; set; }
        public int librarySectionID { get; set; }
        public string type { get; set; }
        public string title { get; set; }
        public string grandparentKey { get; set; }
        public string parentKey { get; set; }
        public string grandparentTitle { get; set; }
        public string parentTitle { get; set; }
        public string summary { get; set; }
        public int index { get; set; }
        public int parentIndex { get; set; }
        public int ratingCount { get; set; }
        public string thumb { get; set; }
        public string art { get; set; }
        public string parentThumb { get; set; }
        public string grandparentThumb { get; set; }
        public string grandparentArt { get; set; }
        public int addedAt { get; set; }
        public int updatedAt { get; set; }
    }

    public class PlexPlayer
    {
        public bool local { get; set; }
        public string publicAddress { get; set; }
        public string title { get; set; }
        public string uuid { get; set; }
    }

    public class PlexServer
    {
        public string title { get; set; }
        public string uuid { get; set; }
    }
}


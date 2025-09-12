using Newtonsoft.Json;

namespace Trackster.Api.Features.Webhooks.Types;

public class PlexWebhookRequest
{
    [JsonProperty("event")]
    public string Event { get; set; }

    [JsonProperty("user")]
    public bool User { get; set; }

    [JsonProperty("owner")]
    public bool Owner { get; set; }

    [JsonProperty("Account")]
    public Account Account { get; set; }

    [JsonProperty("Server")]
    public Server Server { get; set; }

    [JsonProperty("Player")]
    public Player Player { get; set; }

    [JsonProperty("Metadata")]
    public Metadata Metadata { get; set; }
}

public class Account
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("thumb")]
    public string Thumb { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }
}

public class Director
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("filter")]
    public string Filter { get; set; }

    [JsonProperty("tag")]
    public string Tag { get; set; }

    [JsonProperty("tagKey")]
    public string TagKey { get; set; }
}

public class GuidPlex
{
    [JsonProperty("id")]
    public string Id { get; set; }
}

public class Image
{
    [JsonProperty("alt")]
    public string Alt { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }
}

public class Metadata
{
    [JsonProperty("librarySectionType")]
    public string LibrarySectionType { get; set; }

    [JsonProperty("ratingKey")]
    public string RatingKey { get; set; }

    [JsonProperty("key")]
    public string Key { get; set; }

    [JsonProperty("parentRatingKey")]
    public string ParentRatingKey { get; set; }

    [JsonProperty("grandparentRatingKey")]
    public string GrandparentRatingKey { get; set; }

    [JsonProperty("guid")]
    public string Reference { get; set; }

    [JsonProperty("parentGuid")]
    public string ParentGuid { get; set; }

    [JsonProperty("grandparentGuid")]
    public string GrandparentGuid { get; set; }

    [JsonProperty("grandparentSlug")]
    public string GrandparentSlug { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("grandparentKey")]
    public string GrandparentKey { get; set; }

    [JsonProperty("parentKey")]
    public string ParentKey { get; set; }

    [JsonProperty("librarySectionTitle")]
    public string LibrarySectionTitle { get; set; }

    [JsonProperty("librarySectionID")]
    public int LibrarySectionID { get; set; }

    [JsonProperty("librarySectionKey")]
    public string LibrarySectionKey { get; set; }

    [JsonProperty("grandparentTitle")]
    public string GrandparentTitle { get; set; }

    [JsonProperty("parentTitle")]
    public string ParentTitle { get; set; }

    [JsonProperty("contentRating")]
    public string ContentRating { get; set; }

    [JsonProperty("summary")]
    public string Summary { get; set; }

    [JsonProperty("index")]
    public int Index { get; set; }

    [JsonProperty("parentIndex")]
    public int ParentIndex { get; set; }

    [JsonProperty("skipCount")]
    public int SkipCount { get; set; }

    [JsonProperty("year")]
    public int Year { get; set; }

    [JsonProperty("thumb")]
    public string Thumb { get; set; }

    [JsonProperty("art")]
    public string Art { get; set; }

    [JsonProperty("parentThumb")]
    public string ParentThumb { get; set; }

    [JsonProperty("grandparentThumb")]
    public string GrandparentThumb { get; set; }

    [JsonProperty("grandparentArt")]
    public string GrandparentArt { get; set; }

    [JsonProperty("grandparentTheme")]
    public string GrandparentTheme { get; set; }

    [JsonProperty("duration")]
    public int Duration { get; set; }

    [JsonProperty("viewOffset")]
    public int ViewOffsetInMilliseconds { get; set; }

    [JsonProperty("lastViewedAt")]
    public int LastViewedAt { get; set; }

    [JsonProperty("originallyAvailableAt")]
    public string OriginallyAvailableAt { get; set; }

    [JsonProperty("addedAt")]
    public int AddedAt { get; set; }

    [JsonProperty("updatedAt")]
    public int UpdatedAt { get; set; }

    [JsonProperty("chapterSource")]
    public string ChapterSource { get; set; }

    [JsonProperty("Image")]
    public List<Image> Image { get; set; }

    [JsonProperty("UltraBlurColors")]
    public UltraBlurColors UltraBlurColors { get; set; }

    [JsonProperty("Guid")]
    public List<GuidPlex> Guid { get; set; }

    [JsonProperty("Director")]
    public List<Director> Director { get; set; }

    [JsonProperty("Writer")]
    public List<Writer> Writer { get; set; }

    [JsonProperty("Role")]
    public List<Role> Role { get; set; }

    [JsonProperty("Producer")]
    public List<Producer> Producer { get; set; }
}

public class Player
{
    [JsonProperty("local")]
    public bool Local { get; set; }

    [JsonProperty("publicAddress")]
    public string PublicAddress { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("uuid")]
    public string Uuid { get; set; }
}

public class Producer
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("filter")]
    public string Filter { get; set; }

    [JsonProperty("tag")]
    public string Tag { get; set; }

    [JsonProperty("tagKey")]
    public string TagKey { get; set; }

    [JsonProperty("thumb")]
    public string Thumb { get; set; }
}

public class Role
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("filter")]
    public string Filter { get; set; }

    [JsonProperty("tag")]
    public string Tag { get; set; }

    [JsonProperty("tagKey")]
    public string TagKey { get; set; }

    [JsonProperty("role")]
    public string RoleName { get; set; }

    [JsonProperty("thumb")]
    public string Thumb { get; set; }
}

public class Server
{
    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("uuid")]
    public string Uuid { get; set; }
}

public class UltraBlurColors
{
    [JsonProperty("topLeft")]
    public string TopLeft { get; set; }

    [JsonProperty("topRight")]
    public string TopRight { get; set; }

    [JsonProperty("bottomRight")]
    public string BottomRight { get; set; }

    [JsonProperty("bottomLeft")]
    public string BottomLeft { get; set; }
}

public class Writer
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("filter")]
    public string Filter { get; set; }

    [JsonProperty("tag")]
    public string Tag { get; set; }

    [JsonProperty("tagKey")]
    public string TagKey { get; set; }

    [JsonProperty("thumb")]
    public string Thumb { get; set; }
}


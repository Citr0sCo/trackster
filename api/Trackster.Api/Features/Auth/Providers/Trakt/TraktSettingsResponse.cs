using Newtonsoft.Json;

namespace Trackster.Api.Features.Auth.Providers.Trakt;

public class TraktSettingsResponse
{
    [JsonProperty("user")]
    public TraktUser User { get; set; }

    [JsonProperty("account")]
    public TraktAccount Account { get; set; }

    [JsonProperty("connections")]
    public TraktConnections Connections { get; set; }

    [JsonProperty("sharing_text")]
    public TraktSharingText SharingText { get; set; }

    [JsonProperty("limits")]
    public TraktLimits Limits { get; set; }

    [JsonProperty("permissions")]
    public TraktPermissions Permissions { get; set; }
    
    public class TraktAccount
    {
        [JsonProperty("timezone")]
        public string Timezone { get; set; }

        [JsonProperty("date_format")]
        public string DateFormat { get; set; }

        [JsonProperty("time_24hr")]
        public bool Time24hr { get; set; }

        [JsonProperty("cover_image")]
        public string CoverImage { get; set; }
    }

    public class TraktAvatar
    {
        [JsonProperty("full")]
        public string Full { get; set; }
    }

    public class TraktCollection
    {
        [JsonProperty("item_count")]
        public int ItemCount { get; set; }
    }

    public class TraktConnections
    {
        [JsonProperty("facebook")]
        public bool Facebook { get; set; }

        [JsonProperty("twitter")]
        public bool Twitter { get; set; }

        [JsonProperty("mastodon")]
        public bool Mastodon { get; set; }

        [JsonProperty("google")]
        public bool Google { get; set; }

        [JsonProperty("tumblr")]
        public bool Tumblr { get; set; }

        [JsonProperty("medium")]
        public bool Medium { get; set; }

        [JsonProperty("slack")]
        public bool Slack { get; set; }

        [JsonProperty("apple")]
        public bool Apple { get; set; }

        [JsonProperty("dropbox")]
        public bool Dropbox { get; set; }

        [JsonProperty("microsoft")]
        public bool Microsoft { get; set; }
    }

    public class TraktFavorites
    {
        [JsonProperty("item_count")]
        public int ItemCount { get; set; }
    }

    public class TraktIds
    {
        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("uuid")]
        public string Uuid { get; set; }
    }

    public class Images
    {
        [JsonProperty("avatar")]
        public TraktAvatar Avatar { get; set; }
    }

    public class TraktLimits
    {
        [JsonProperty("list")]
        public TraktList List { get; set; }

        [JsonProperty("watchlist")]
        public TraktWatchlist Watchlist { get; set; }

        [JsonProperty("favorites")]
        public TraktFavorites Favorites { get; set; }

        [JsonProperty("search")]
        public TraktSearch Search { get; set; }

        [JsonProperty("collection")]
        public TraktCollection Collection { get; set; }

        [JsonProperty("notes")]
        public TraktNotes Notes { get; set; }
    }

    public class TraktList
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("item_count")]
        public int ItemCount { get; set; }
    }

    public class TraktNotes
    {
        [JsonProperty("item_count")]
        public int ItemCount { get; set; }
    }

    public class TraktPermissions
    {
        [JsonProperty("commenting")]
        public bool Commenting { get; set; }

        [JsonProperty("liking")]
        public bool Liking { get; set; }

        [JsonProperty("following")]
        public bool Following { get; set; }
    }

    public class TraktSearch
    {
        [JsonProperty("recent_count")]
        public int RecentCount { get; set; }
    }

    public class TraktSharingText
    {
        [JsonProperty("watching")]
        public string Watching { get; set; }

        [JsonProperty("watched")]
        public string Watched { get; set; }

        [JsonProperty("rated")]
        public string Rated { get; set; }
    }

    public class TraktUser
    {
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("private")]
        public bool Private { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("vip")]
        public bool Vip { get; set; }

        [JsonProperty("vip_ep")]
        public bool VipEp { get; set; }

        [JsonProperty("ids")]
        public TraktIds Ids { get; set; }

        [JsonProperty("joined_at")]
        public DateTime JoinedAt { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("about")]
        public string About { get; set; }

        [JsonProperty("gender")]
        public string Gender { get; set; }

        [JsonProperty("age")]
        public int Age { get; set; }

        [JsonProperty("images")]
        public Images Images { get; set; }

        [JsonProperty("vip_og")]
        public bool VipOg { get; set; }

        [JsonProperty("vip_years")]
        public int VipYears { get; set; }
    }

    public class TraktWatchlist
    {
        [JsonProperty("item_count")]
        public int ItemCount { get; set; }
    }
}

using Newtonsoft.Json;
using Trackster.Api.Data.Records;

namespace Trackster.Api.Features.Media.Importers.TmdbImporter.Types;

public class TmdbShowDetails
{
    public TmdbShowDetails()
    {
        Seasons = new List<Season>();
    }
    
    [JsonProperty("id")] 
    public int Identifier { get; set; }
    
    [JsonProperty("name")] 
    public string Title { get; set; }
    
    [JsonProperty("poster_path")] 
    public string PosterUrl { get; set; }
    
    [JsonProperty("overview")] 
    public string Overview { get; set; }
    
    [JsonProperty("seasons")] 
    public List<Season> Seasons { get; set; }
    
    [JsonProperty("first_air_date")]
    public DateTime FirstAirDate { get; set; }
    
    //public bool adult { get; set; }
    //public string backdrop_path { get; set; }
    //public List<CreatedBy> created_by { get; set; }
    //public List<int> episode_run_time { get; set; }
    //public List<Genre> genres { get; set; }
    //public string homepage { get; set; }
    //public bool in_production { get; set; }
    //public List<string> languages { get; set; }
    //public string last_air_date { get; set; }
    //public LastEpisodeToAir last_episode_to_air { get; set; }
    //public object next_episode_to_air { get; set; }
    //public List<Network> networks { get; set; }
    //public int number_of_episodes { get; set; }
    //public int number_of_seasons { get; set; }
    //public List<string> origin_country { get; set; }
    //public string original_language { get; set; }
    //public string original_name { get; set; }
    //public double popularity { get; set; }
    //public List<ProductionCompany> production_companies { get; set; }
    //public List<ProductionCountry> production_countries { get; set; }
    //public List<SpokenLanguage> spoken_languages { get; set; }
    //public string status { get; set; }
    //public string tagline { get; set; }
    //public string type { get; set; }
    //public double vote_average { get; set; }
    //public int vote_count { get; set; }
    
    public class CreatedBy
    {
        public int id { get; set; }
        public string credit_id { get; set; }
        public string name { get; set; }
        public string original_name { get; set; }
        public int gender { get; set; }
        public string profile_path { get; set; }
    }

    public class Genre
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class LastEpisodeToAir
    {
        public int id { get; set; }
        public string name { get; set; }
        public string overview { get; set; }
        public double vote_average { get; set; }
        public int vote_count { get; set; }
        public string air_date { get; set; }
        public int episode_number { get; set; }
        public string episode_type { get; set; }
        public string production_code { get; set; }
        public int runtime { get; set; }
        public int season_number { get; set; }
        public int show_id { get; set; }
        public string still_path { get; set; }
    }

    public class Network
    {
        public int id { get; set; }
        public string logo_path { get; set; }
        public string name { get; set; }
        public string origin_country { get; set; }
    }

    public class ProductionCompany
    {
        public int id { get; set; }
        public string logo_path { get; set; }
        public string name { get; set; }
        public string origin_country { get; set; }
    }

    public class ProductionCountry
    {
        public string iso_3166_1 { get; set; }
        public string name { get; set; }
    }

    public class Season
    {
        [JsonProperty("id")]
        public int Identifier { get; set; }
        
        [JsonProperty("name")]
        public string Title { get; set; }
        
        public string air_date { get; set; }
        public int episode_count { get; set; }
        public string overview { get; set; }
        public string poster_path { get; set; }
        public int season_number { get; set; }
        public double vote_average { get; set; }
    }

    public class SpokenLanguage
    {
        public string english_name { get; set; }
        public string iso_639_1 { get; set; }
        public string name { get; set; }
    }
}
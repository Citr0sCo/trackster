using Newtonsoft.Json;

namespace Trackster.Api.Features.Media.Importers.TmdbImporter.Types;

public class TmdbMovieDetails
{
    [JsonProperty("id")]
    public int Identifier { get; set; }
    
    [JsonProperty("imdb_id")]
    public string IMDB { get; set; }
    
    [JsonProperty("adult")]
    public bool IsAdult { get; set; }
    
    [JsonProperty("title")]
    public string Title { get; set; }
    
    [JsonProperty("overview")]
    public string Overview { get; set; }
    
    [JsonProperty("poster_path")]
    public string PosterUrl { get; set; }
    
    [JsonProperty("release_date")]
    public string ReleaseDate { get; set; }
    
    [JsonProperty("genres")]
    public List<Genre> Genres { get; set; }
    
    public string backdrop_path { get; set; }
    public object belongs_to_collection { get; set; }
    public int budget { get; set; }
    public string homepage { get; set; }
    public List<string> origin_country { get; set; }
    public string original_language { get; set; }
    public string original_title { get; set; }
    public double popularity { get; set; }
    public List<ProductionCompany> production_companies { get; set; }
    public List<ProductionCountry> production_countries { get; set; }
    public int revenue { get; set; }
    public int runtime { get; set; }
    public List<SpokenLanguage> spoken_languages { get; set; }
    public string status { get; set; }
    public string tagline { get; set; }
    public bool video { get; set; }
    public double vote_average { get; set; }
    public int vote_count { get; set; }
    
    public class Genre
    {
        [JsonProperty("id")]
        public int Identifier { get; set; }
        
        [JsonProperty("name")]
        public string Name { get; set; }
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

    public class SpokenLanguage
    {
        public string english_name { get; set; }
        public string iso_639_1 { get; set; }
        public string name { get; set; }
    }
}
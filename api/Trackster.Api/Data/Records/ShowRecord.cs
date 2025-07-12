using System.ComponentModel.DataAnnotations;

namespace Trackster.Api.Data.Records;

public class ShowRecord
{
    [Key]
    public Guid Identifier { get; set; }

    public string Title { get; set; }
    public int Year { get; set; }
    public string TMDB { get; set; }
    public string? Poster { get; set; }
    public string? Overview { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace Trackster.Api.Data.Records;

public class MovieGenreRecord
{
    [Key]
    public Guid Identifier { get; set; }
    
    public Guid MovieId { get; set; }
    public MovieRecord Movie { get; set; }

    public Guid GenreId { get; set; }
    public GenreRecord Genre { get; set; }
}
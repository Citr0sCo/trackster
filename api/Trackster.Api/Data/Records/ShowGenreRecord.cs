using System.ComponentModel.DataAnnotations;

namespace Trackster.Api.Data.Records;

public class ShowGenreRecord
{
    [Key]
    public Guid Identifier { get; set; }
    
    public Guid ShowId { get; set; }
    public ShowRecord Show { get; set; }
    
    public Guid GenreId { get; set; }
    public GenreRecord Genre { get; set; }
}
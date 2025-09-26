using System.ComponentModel.DataAnnotations;

namespace Trackster.Api.Data.Records;

public class GenreRecord
{
    [Key]
    public Guid Identifier { get; set; }

    public string Name { get; set; }
}
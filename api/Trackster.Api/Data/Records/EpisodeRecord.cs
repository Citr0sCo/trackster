using System.ComponentModel.DataAnnotations;

namespace Trackster.Api.Data.Records;

public class EpisodeRecord
{
    [Key]
    public Guid Identifier { get; set; }
    public int Number { get; set; }
    public SeasonRecord Season { get; set; }
}
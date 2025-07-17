using System.ComponentModel.DataAnnotations;

namespace Trackster.Api.Data.Records;

public class SeasonRecord
{
    [Key]
    public Guid Identifier { get; set; }
    public int Number { get; set; }
    public ShowRecord Show { get; set; }
    public string Title { get; set; }
}
namespace HomeBoxLanding.Api.Features.Radarr.Types;

public class RadarrActivityResponse
{
    public RadarrActivityResponse()
    {
        Health = new List<RadarrHealth>();
    }
    
    public int TotalNumberOfMovies { get; set; }
    public int TotalNumberOfQueuedMovies { get; set; }
    public int TotalMissingMovies { get; set; }
    public List<RadarrHealth> Health { get; set; }
}
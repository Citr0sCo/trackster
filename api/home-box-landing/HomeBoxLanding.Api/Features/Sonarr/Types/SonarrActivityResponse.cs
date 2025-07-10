namespace HomeBoxLanding.Api.Features.Sonarr.Types;

public class SonarrActivityResponse
{
    public SonarrActivityResponse()
    {
        Health = new List<SonarrHealth>();
    }
    
    public int TotalNumberOfSeries { get; set; }
    public int TotalNumberOfQueuedEpisodes { get; set; }
    public int TotalNumberOfMissingEpisodes { get; set; }
    public List<SonarrHealth> Health { get; set; }
}
namespace Trackster.Api.Features.Users.Types;

public class User
{
    public User()
    {
        ThirdPartyIntegrations = new List<ThirdPartyIntegration>();
    }
    
    public Guid Identifier { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<ThirdPartyIntegration> ThirdPartyIntegrations { get; set; }
}
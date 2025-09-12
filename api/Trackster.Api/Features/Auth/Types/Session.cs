namespace Trackster.Api.Features.Auth.Types;

public class Session
{
    private readonly Guid _sessionReference;
    private readonly Guid _userReference;
    private DateTime _ttl;

    public Session(Guid sessionReference, Guid userReference)
    {
        _sessionReference = sessionReference;
        _userReference = userReference;
        _ttl  = DateTime.Now.AddMinutes(30);
    }

    public Guid Reference()
    {
        return _sessionReference;
    }

    public Guid UserIdentifier()
    {
        return _userReference;
    }

    public DateTime TimeToLive()
    {
        return _ttl;
    }

    public void ExtendTimeToLive()
    {
        _ttl = _ttl.AddMinutes(30);
    }

    public bool Expired()
    {
        return _ttl < DateTime.Now;
    }
}
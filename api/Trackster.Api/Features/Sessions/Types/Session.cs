using Trackster.Api.Data.Records;

namespace Trackster.Api.Features.Sessions.Types;

public class Session
{
    private readonly Guid _sessionReference;
    private readonly Guid _userReference;
    private DateTime _ttl;

    public Session(Guid sessionReference, Guid userReference, bool remember)
    {
        _sessionReference = sessionReference;
        _userReference = userReference;
        _ttl  = remember ? DateTime.Now.AddDays(30) : DateTime.Now.AddMinutes(30);
    }
    
    public Session(SessionRecord record)
    {
        _sessionReference = record.Identifier;
        _userReference = record.User.Identifier;
        _ttl  = record.TimeToLive;
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
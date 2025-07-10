using System.Net.WebSockets;

namespace HomeBoxLanding.Api.Features.WebSockets;

public class InternalWebSocket
{
    public DateTime LastSeen { get; set; }

    private readonly WebSocket _socket;

    public InternalWebSocket(WebSocket socket)
    {
        _socket = socket;
    }
        
    public bool HasDisconnected()
    {
        return (LastSeen - DateTime.Now).Minutes > 2;
    }

    public void SendAsync(ArraySegment<byte> data, WebSocketMessageType messageType, WebSocketMessageFlags flags, CancellationToken cancellationToken)
    {
        if (_socket.State == WebSocketState.Closed)
            return;
            
        _socket.SendAsync(data, messageType, flags, cancellationToken);
        LastSeen = DateTime.Now;
    }

    public void Close(string reason, CancellationToken cancellationToken)
    {
        _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, reason, cancellationToken);
    }
}
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HomeBoxLanding.Api.Features.WebSockets;

public class WebSocketController : ControllerBase
{
    private readonly IWebSocketManager _webSocketManager;

    public WebSocketController()
    {
        _webSocketManager = WebSocketManager.Instance();
    }

    [Route("/ws")]
    public async Task Get()
    {
        try 
        {
            if (_webSocketManager.CancellationToken().IsCancellationRequested)
                return;
            
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using (var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync())
                {
                    var buffer = new byte[1024 * 4];
                    var receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _webSocketManager.CancellationToken());
                    var sessionId = Guid.NewGuid();

                    while (receiveResult.CloseStatus.HasValue == false)
                    {
                        _webSocketManager.Receive(sessionId, buffer, webSocket);
                        receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _webSocketManager.CancellationToken());
                    }

                    await webSocket.CloseAsync(receiveResult.CloseStatus.Value, receiveResult.CloseStatusDescription, _webSocketManager.CancellationToken());
                }
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("An unknown exception occured whilst receiving a socket request. Exception below:");
            Console.WriteLine(e.Message);
            Console.WriteLine(JsonConvert.SerializeObject(e));
        }
    }
}
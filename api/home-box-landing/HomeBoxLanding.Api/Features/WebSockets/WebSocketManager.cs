using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using HomeBoxLanding.Api.Core.Events.Types;
using HomeBoxLanding.Api.Features.WebSockets.Types;
using Microsoft.AspNetCore.Connections;
using Newtonsoft.Json;

namespace HomeBoxLanding.Api.Features.WebSockets;

public interface IWebSocketManager : ISubscriber
{
    void Add(Guid sessionId, InternalWebSocket socket);
    void Send(Guid sessionId, WebSocketKey key, object data);
    void Receive(Guid sessionId, byte[] socketMessage, WebSocket webSocket);
    void Close(Guid sessionId);
    void SendToAllClients(WebSocketKey key, object data);
    void CloseAll();
    CancellationToken CancellationToken();
}

public class WebSocketManager : IWebSocketManager
{
    private readonly ConcurrentDictionary<Guid, InternalWebSocket> _clients;
    private static IWebSocketManager? _instance;
    private static bool _isRunning = true;
    private readonly CancellationTokenSource _cancellationTokenSource;

    private WebSocketManager()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        
        _clients = new ConcurrentDictionary<Guid, InternalWebSocket>();

        Task.Run(() =>
        {
            while (_isRunning)
            {
                foreach (var client in _clients)
                {
                    if (client.Value.HasDisconnected())
                        _instance?.Close(client.Key);
                }

                Thread.Sleep(5000);
            }
        }, _cancellationTokenSource.Token);
    }

    public CancellationToken CancellationToken()
    {
        return _cancellationTokenSource.Token;
    }

    public static IWebSocketManager Instance()
    {
        if (_instance == null)
            _instance = new WebSocketManager();

        return _instance;
    }

    public void Add(Guid sessionId, InternalWebSocket socket)
    {
        try
        {
            _clients.TryAdd(sessionId, socket);
            Console.WriteLine($"Added client {sessionId}.");
        }
        catch (WebSocketException e)
        {
            Console.WriteLine("An Web Socket Exception occured whilst adding a socket to a manager.");
            Console.WriteLine(e.Message);
            Console.WriteLine(JsonConvert.SerializeObject(e));
        }
        catch (TaskCanceledException e)
        {
            if (!(e.InnerException is ConnectionAbortedException))
                Console.WriteLine("An unknown exception occured whilst adding a socket to a manager.");
            Console.WriteLine(e.Message);
            Console.WriteLine(JsonConvert.SerializeObject(e));
        }
        catch (Exception e)
        {
            Console.WriteLine("An unknown exception occured whilst adding a socket to a manager.");
            Console.WriteLine(e.Message);
            Console.WriteLine(JsonConvert.SerializeObject(e));
        }
    }

    public void Update(Guid sessionId, InternalWebSocket socket)
    {
        try
        {
            _clients.TryRemove(sessionId, out var existingSocket);
            _clients.TryAdd(sessionId, socket);

            Console.WriteLine($"Updated client {sessionId}.");
        }
        catch (WebSocketException e)
        {
            Console.WriteLine("An Web Socket Exception occured whilst adding a socket to a manager. Exception below:");
            Console.WriteLine(e.Message);
            Console.WriteLine(JsonConvert.SerializeObject(e));
        }
        catch (TaskCanceledException e)
        {
            if (!(e.InnerException is ConnectionAbortedException))
            {
                Console.WriteLine("An unknown exception occured whilst adding a socket to a manager. Exception below:");
                Console.WriteLine(e.Message);
                Console.WriteLine(JsonConvert.SerializeObject(e));
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("An unknown exception occured whilst adding a socket to a manager. Exception below:");
            Console.WriteLine(e.Message);
            Console.WriteLine(JsonConvert.SerializeObject(e));
        }
    }

    public void Send(Guid sessionId, WebSocketKey key, object data)
    {
        try
        {
            if (!_clients.TryGetValue(sessionId, out var client))
                return;

            var serializedMessage = JsonConvert.SerializeObject(new CommonSocketMessageResponse
            {
                Key = key.ToString(),
                Data = data
            });

            Console.WriteLine(JsonConvert.SerializeObject(client));

            client.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(serializedMessage), 0, serializedMessage.Length), WebSocketMessageType.Text, WebSocketMessageFlags.EndOfMessage, _cancellationTokenSource.Token);
            Console.WriteLine($"Sent message to client {sessionId}.");
        }
        catch (WebSocketException e)
        {
            Console.WriteLine("An Web Socket Exception occured whilst sending a message to a socket. Exception below:");
            Console.WriteLine(e.Message);
            Console.WriteLine(JsonConvert.SerializeObject(e));
        }
        catch (TaskCanceledException e)
        {
            if (!(e.InnerException is ConnectionAbortedException))
            {
                Console.WriteLine("An unknown exception occured whilst sending a message to a socket. Exception below:");
                Console.WriteLine(e.Message);
                Console.WriteLine(JsonConvert.SerializeObject(e));
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("An unknown exception occured whilst sending a message to a socket. Exception below:");
            Console.WriteLine(e.Message);
            Console.WriteLine(JsonConvert.SerializeObject(e));
        }
    }

    public void Receive(Guid sessionId, byte[] socketMessage, WebSocket webSocket)
    {
        try
        {
            var currentSessionId = Guid.Empty;
            var message = JsonConvert.DeserializeObject<CommonSocketMessageRequest>(Encoding.UTF8.GetString(socketMessage));

            if (message?.SessionId != null && _clients.TryGetValue((Guid)message.SessionId, out var client))
            {
                Update((Guid)message.SessionId, new InternalWebSocket(webSocket) { LastSeen = DateTime.Now });
                currentSessionId = (Guid)message.SessionId;
            }
            else
            {
                Add(sessionId, new InternalWebSocket(webSocket) { LastSeen = DateTime.Now });
                currentSessionId = sessionId;
            }

            if (message?.Key == WebSocketKey.Handshake.ToString())
                Send(currentSessionId, WebSocketKey.Handshake, currentSessionId);

            Update(currentSessionId, new InternalWebSocket(webSocket) { LastSeen = DateTime.Now });

            Console.WriteLine("Received message from client:", JsonConvert.SerializeObject(message?.Data));
        }
        catch (WebSocketException e)
        {
            Console.WriteLine("An Web Socket Exception occured whilst receiving from a socket to a manager. Exception below:");
            Console.WriteLine(e.Message);
            Console.WriteLine(JsonConvert.SerializeObject(e));
        }
        catch (TaskCanceledException e)
        {
            if (!(e.InnerException is ConnectionAbortedException))
            {
                Console.WriteLine("An unknown exception occured whilst receiving from a socket to a manager. Exception below:");
                Console.WriteLine(e.Message);
                Console.WriteLine(JsonConvert.SerializeObject(e));
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("An unknown exception occured whilst receiving from a socket to a manager. Exception below:");
            Console.WriteLine(e.Message);
            Console.WriteLine(JsonConvert.SerializeObject(e));
        }
    }

    public void Close(Guid sessionId)
    {
        try
        {
            if (!_clients.TryRemove(sessionId, out var webSocket))
                return;

            webSocket.Close("Regular closure.", _cancellationTokenSource.Token);
        }
        catch (WebSocketException e)
        {
            Console.WriteLine("An Web Socket Exception occured whilst removing a socket to a manager. Exception below:");
            Console.WriteLine(e.Message);
            Console.WriteLine(JsonConvert.SerializeObject(e));
        }
        catch (TaskCanceledException e)
        {
            if (!(e.InnerException is ConnectionAbortedException))
            {
                Console.WriteLine("An unknown exception occured whilst removing a socket from the manager. Exception below:");
                Console.WriteLine(e.Message);
                Console.WriteLine(JsonConvert.SerializeObject(e));
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("An unknown exception occured whilst removing a socket from the manager. Exception below:");
            Console.WriteLine(e.Message);
            Console.WriteLine(JsonConvert.SerializeObject(e));
        }
    }

    public void SendToAllClients(WebSocketKey key, object data)
    {
        try 
        {
            foreach (var client in _clients.Values)
            {
                var serializedMessage = JsonConvert.SerializeObject(new CommonSocketMessageResponse
                {
                    Key = key.ToString(),
                    Data = data
                });
                client.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(serializedMessage), 0, serializedMessage.Length), WebSocketMessageType.Text, WebSocketMessageFlags.EndOfMessage, _cancellationTokenSource.Token);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("An unknown exception occured whilst sending data to all clients. Exception below:");
            Console.WriteLine(e.Message);
            Console.WriteLine(JsonConvert.SerializeObject(e));
        }
    }

    public void CloseAll()
    {
        foreach (var client in _clients.Values)
        {
            client.Close("Closing all.", _cancellationTokenSource.Token);
        }
    }

    public void OnStarted()
    {
        _isRunning = true;
    }

    public void OnStopping()
    {
        CloseAll();
        _cancellationTokenSource.Cancel();
        _isRunning = false;
    }

    public void OnStopped()
    {
        // Do nothing
    }
}
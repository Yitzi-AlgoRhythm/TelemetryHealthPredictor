using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace TelemetryHealthPredictor.Services.Connection
{
    public class WebsocketService
    {
        private readonly ConcurrentBag<WebSocket> _sockets;

        public WebsocketService()
        {
            _sockets = new ConcurrentBag<WebSocket>();
        }

        public void AddClient(WebSocket clientSocket)
        {
            _sockets.Add(clientSocket);
        }

        public void BroadcastResults()
        {
            // function for TPL ActionBlock that broadcasts the health score, and any alerts, to all open websockets
            // receive results as params and SendAsync them
        }
    }
}
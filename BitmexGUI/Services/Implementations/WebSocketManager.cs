using System.Net.WebSockets;


namespace BitmexGUI.Services.Implementations
{
    internal class WebSocketManager
    {
        private static WebSocketManager _instance;
        private static readonly object _lock = new object();
        private readonly List<WebSocket> _webSockets = new List<WebSocket>();

        // Singleton pattern
        public static WebSocketManager Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new WebSocketManager();
                    }
                    return _instance;
                }
            }
        }

        private WebSocketManager() { }

        // Add a WebSocket connection
        public void AddWebSocket(WebSocket webSocket)
        {
            lock (_lock)
            {
                _webSockets.Add(webSocket);
            }
        }

        // Remove a WebSocket connection
        public void RemoveWebSocket(WebSocket webSocket)
        {
            lock (_lock)
            {
                _webSockets.Remove(webSocket);
            }
        }

        // Close all WebSocket connections
        public async Task CloseAllWebSocketsAsync(CancellationToken cancellationToken)
        {
            List<Task> closeTasks;
            lock (_lock)
            {
                closeTasks = new List<Task>();
                foreach (var webSocket in _webSockets)
                {
                    if (webSocket.State == WebSocketState.Open)
                    {
                        closeTasks.Add(webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", cancellationToken));
                    }
                }

            }

            try
            {
                await Task.WhenAll(closeTasks);
            }
            catch (Exception ex)
            {
                // Log or handle exceptions that occurred during the closure
                Console.WriteLine($"Exception during WebSocket closure: {ex.Message}");
            }
        }

        public List<WebSocket> GetAllWebSockets()
        {
            return _webSockets;
        }
    }

}


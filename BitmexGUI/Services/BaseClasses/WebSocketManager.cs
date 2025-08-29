using System.Net.WebSockets;


namespace BitmexGUI.Services.Implementations
{
    public class WebSocketManager
    {
        private static WebSocketManager _instance;
        private static readonly object _lock = new object();
        private readonly Dictionary<string,WebSocket> _webSockets = new Dictionary<string, WebSocket>();

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

        private WebSocketManager() 
        {

        }

        // Add a WebSocket connection
        public void AddWebSocket(string name,WebSocket webSocket)
        {
            lock (_lock)
            {
                if (!_webSockets.ContainsKey(name))
                {
                    _webSockets.Add(name, webSocket);
                }
                    
            }
        }

        // Remove a WebSocket connection
        public void RemoveWebSocket(string name, WebSocket webSocket)
        {
            lock (_lock)
            {
                if (_webSockets.ContainsKey(name))
                    _webSockets.Remove(name);
            }
        }

        // Close all WebSocket connections
        public async Task CloseAllWebSocketsAsync()
        {
            List<Task> closeTasks;
             
            lock (_lock)
            {
                closeTasks = new List<Task>();
                foreach (KeyValuePair<string,WebSocket> webSocket in _webSockets)
                {
                    if (webSocket.Key.Contains("Price") && _webSockets[webSocket.Key].State == WebSocketState.Open)
                    {
                        closeTasks.Add(_webSockets[webSocket.Key].CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", BitmexAPI.ConnectionTokenSource.Token));
                        _webSockets.Remove(webSocket.Key);
                    }
                }

            }

            try
            { 
                await Task.WhenAll(closeTasks);
                 //BitmexAPI.ReceiveTokenSource.Dispose();
                 //BitmexAPI.ConnectionTokenSource.Dispose();
            }
            catch (Exception ex)
            {
                // Log or handle exceptions that occurred during the closure
                Console.WriteLine($"Exception during WebSocket closure: {ex.Message}");
            }
        }

        
    }

}


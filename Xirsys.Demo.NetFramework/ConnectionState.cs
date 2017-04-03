using System;
using System.Collections.Concurrent;
using System.Threading;
using AIMLbot;
using WebSocket4Net;

namespace Xirsys.Demo
{
    public class ConnectionState
    {
        public String WebSocketUri { get; set; }
        public WebSocket CurrentWebSocket { get; set; }
        public bool StopReconnect { get; set; }

        public CancellationTokenSource CancelTokenSource { get; set; }

        public Bot Bot { get; set; }
        public ConcurrentDictionary<String, User> BotUsers { get; set; }
    }
}

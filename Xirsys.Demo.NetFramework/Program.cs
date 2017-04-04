using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AIMLbot;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NLog.Extensions.Logging;
using WebSocket4Net;
using Xirsys.Client;
using Xirsys.Client.Models.WebSocket;
using Xirsys.Client.Models.WebSocket.Payloads;
using Xirsys.Client.Serialization;
using Xirsys.Client.Utilities;
using Xirsys.Demo.NetFramework.Extensions;

namespace Xirsys.Demo.NetFramework
{
    class Program
    {
        private static readonly ILoggerFactory LoggerFactory;
        private static readonly ILogger Log;
        private static readonly IFormatProvider FormatProvider;

        private static readonly TimeSpan RECONNECT_DELAY = TimeSpan.FromSeconds(3);

        private static IConfiguration Configuration;

        static Program()
        {
            LoggerFactory = new LoggerFactory();
            LoggerFactory.AddNLog();
            Log = LoggerFactory.CreateLogger<Program>();
            FormatProvider = JsonFormatProvider.Current;
        }

        static void Main(String[] args)
        {
            try
            {
                var configurationBuilder = new ConfigurationBuilder();

                configurationBuilder.AddJsonFile("config.json");
                configurationBuilder.AddJsonFile("config.json.user", true);

                Configuration = configurationBuilder.Build();
                
                var t = MainAsync(args);
                t.Wait();
            }
            catch (Exception ex)
            {
                Log.LogError(0, ex, "Exception");
            }
            finally
            {
                Log.LogInformation("Done");
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
            }
        }

        static async Task MainAsync(String[] args)
        {
            await StartBot();
            return;
        }

        private static async Task StartBot()
        {
            // hold reference to all connected users for bot
            var users = new ConcurrentDictionary<String, User>();

            // construct and setup bot
            var kaceyBot = new Bot();
            kaceyBot.loadSettings(@"botconfig\Settings.xml");
            kaceyBot.loadAIMLFromFiles();
            kaceyBot.isAcceptingUserInput = true;

            // setup our initial xirsysclient
            var xirsysClient = new XirsysApiClient(
                (XirsysRegion)Enum.Parse(typeof(XirsysRegion), Configuration["xirsysRegion"]),
                Configuration["xirsysIdent"],
                Configuration["xirsysSecret"], 
                LoggerFactory.CreateLogger<XirsysApiClient>());

            // get url for signal server
            var signalServerUrl = await xirsysClient.GetBestSignalServerAsync();
            if (!signalServerUrl.IsOk())
            {
                return;
            }

            String appPath = Configuration["xirsysPath"];
            String botName = "Kacey";

            // acquire token for bot
            var acquireTokenResponse = await xirsysClient.CreateTokenAsync(appPath, botName, 10000);
            if (!acquireTokenResponse.IsOk())
            {
                return;
            }

            var appSocketUri = signalServerUrl.Data.TrimEnd('/') + $"/v2/{acquireTokenResponse.Data}";
            Log.LogInformation(appSocketUri);

            // holds connection state, which is used via WebSocket events
            var connectionState = new ConnectionState()
                {
                    WebSocketUri = appSocketUri,
                    CancelTokenSource = new CancellationTokenSource(),

                    Bot = kaceyBot,
                    BotUsers = users
                };
            // connect websocket
            StartWebSocket(connectionState, TimeSpan.Zero);

            // console input to shutdown app
            var line = Console.ReadLine();
            while (!String.Equals(line, "exit", StringComparison.InvariantCultureIgnoreCase))
            {
                line = Console.ReadLine();
            }

            Log.LogInformation("Shutting down WebSocket");
            connectionState.StopReconnect = true;
            connectionState.CurrentWebSocket.Close();
        }


        private static void StartWebSocket(ConnectionState state, TimeSpan connectDelay)
        {
            // wait a bit before reconnecting
            if (connectDelay > TimeSpan.Zero)
            {
                Thread.Sleep(RECONNECT_DELAY);
            }

            var ws = new WebSocket(state.WebSocketUri);
            state.CurrentWebSocket = ws;

            ws.Opened += (sender, e) =>
                {
                    Log.LogDebug("WebSocket OnOpen.");

                    // send off bot's welcoming message to chat
                    var messageObj = new BaseWireModel(MessageTypeCode.User, "Hi chat",
                        new Meta(state.Bot.BotName(), OperationType.Message));
                    SendMessage(ws, messageObj);
                };

            ws.Closed += (sender, e) =>
                {
                    Log.LogDebug("WebSocket OnClose.");
                    if (state.StopReconnect)
                    {
                        Log.LogTrace("Not Reconnecting WebSocket");
                        return;
                    }

                    // else reconnect
                    Log.LogTrace("Reconnecting WebSocket");
                    StartWebSocket(state, RECONNECT_DELAY);
                };

            ws.Error += (sender, e) =>
                {
                    Log.LogError(0, e.Exception, "WebSocket OnError.");
                };

            ws.MessageReceived += (sender, e) =>
                {
                    Log.LogDebug("WebSocket OnMessage. Message: {0}", e.Message);

                    // handle incoming messages
                    var botName = state.Bot.BotName();

                    var message = JsonNetExtensions.DeserializeObject<BaseWireModel>(e.Message);
                    if (message.TypeCode != MessageTypeCode.User)
                    {
                        // donno what these are yet
                        Log.LogTrace("Unknown MessageTypeCode: {0}", message.TypeCode);
                        return;
                    }

                    if (message.Meta == null)
                    {
                        Log.LogTrace("Meta IsNull: {0}", message.Meta == null);
                        return;
                    }

                    switch (message.Meta.OperationType)
                    {
                        case OperationType.OnPeers:
                            HandlePeersMessage(message, state);
                            break;
                        case OperationType.PeerConnected:
                            HandlePeerConnectedMessage(message, state);
                            break;
                        case OperationType.PeerRemoved:
                            HandlePeerRemovedMessage(message, state);
                            break;
                        case OperationType.Message:
                            // messages from users (or should be)
                            HandleMessage(message, state);
                            break;
                        case OperationType.Session:
                            Log.LogTrace("Unhandled OperationType: {0}", message.Meta.OperationType);
                            break;
                        case OperationType.Unknown:
                        default:
                            Log.LogTrace("Unknown OperationType: {0}", message.Meta.OperationType);
                            break;
                    }
                };
            
            ws.Open();
            Log.LogTrace("WebSocket Opened");
        }

        private static void SendMessage(WebSocket ws, BaseWireModel model)
        {
            var messageJson = JsonNetExtensions.SerializeObject(model);

            Log.LogTrace("Sending Message: {0}", messageJson);
            ws.Send(messageJson);
        }

        private static void HandlePeersMessage(BaseWireModel data, ConnectionState state)
        {
            Log.LogTrace($"{nameof(HandlePeersMessage)}");
            var userListPayload = data.AsPayload<UserListPayload>();
            // remove duplicates...cause xirsys is terrible
            userListPayload.Users = userListPayload.Users.Distinct().ToList();
            if (Log.IsEnabled(LogLevel.Trace))
            {
                Log.LogTrace("Sent Users: " + String.Join(", ", userListPayload.Users));
            }

            // lets test if all the same users are on since we have reconnected, remove those from botlist
            // that are not
            var removeUsers = state.BotUsers.Keys.Except(userListPayload.Users).ToList();
            foreach (var removeUser in removeUsers)
            {
                Log.LogTrace("Removing UserName ({0}) from BotUsers list", removeUser);
                User ignore;
                state.BotUsers.TryRemove(removeUser, out ignore);
            }

            var botName = state.Bot.BotName();
            // now lets handle any truly new users
            foreach (var newUser in userListPayload.Users.Except(state.BotUsers.Keys).ToList())
            {
                if (String.Equals(newUser, botName))
                {
                    // skip
                    continue;
                }
                var newBotUser = new User(newUser, state.Bot);
                if (state.BotUsers.TryAdd(newUser, newBotUser))
                {
                    Log.LogTrace("Added UserName ({0}) to BotUsers list", newUser);
                }
            }
        }

        // new user connected to websocket
        private static void HandlePeerConnectedMessage(BaseWireModel data, ConnectionState state)
        {
            Log.LogTrace($"{nameof(HandlePeerConnectedMessage)}");
            if (data.Meta.From == null)
            {
                return;
            }

            String botName = state.Bot.BotName();
            String connectedUserName = data.Meta.From;
            var newBotUser = new User(connectedUserName, state.Bot);

            if (!String.Equals(connectedUserName, botName) &&
                state.BotUsers.TryAdd(connectedUserName, newBotUser))
            {
                Log.LogTrace("Added UserName ({0}) to BotUsers list", connectedUserName);

                var messageObj = new BaseWireModel(MessageTypeCode.User, $"Hello {connectedUserName}",
                    new Meta(botName, OperationType.Message));
                SendMessage(state.CurrentWebSocket, messageObj);
            }
        }

        // user left websocket
        private static void HandlePeerRemovedMessage(BaseWireModel data, ConnectionState state)
        {
            Log.LogTrace($"{nameof(HandlePeerRemovedMessage)}");
            if (data.Meta.From != null)
            {
                String disconnectedUserName = data.Meta.From;
                Log.LogTrace("Removing UserName ({0}) from BotUsers list", disconnectedUserName);
                User ignore;
                state.BotUsers.TryRemove(disconnectedUserName, out ignore);
            }
        }

        private static void HandleMessage(BaseWireModel data, ConnectionState state)
        {
            Log.LogTrace($"{nameof(HandleMessage)}");
            if (data.Meta.From == null)
            {
                Log.LogTrace("From property null");
                return;
            }

            User user;
            if (!state.BotUsers.TryGetValue(data.Meta.From, out user))
            {
                // users already disconnected
                Log.LogTrace("UserName ({0}) not found in BotUsers list", data.Meta.From);
                return;
            }

            var payloadMessage = data.Payload as String;
            if (String.IsNullOrWhiteSpace(payloadMessage))
            {
                Log.LogTrace("Empty message received");
                // empty message
                return;
            }

            Request r = new Request(payloadMessage, user, state.Bot);
            Result res = state.Bot.Chat(r);

            var messageObj = new BaseWireModel(MessageTypeCode.User, res.Output,
                new Meta(state.Bot.BotName(), OperationType.Message));
            SendMessage(state.CurrentWebSocket, messageObj);
        }
    }
}

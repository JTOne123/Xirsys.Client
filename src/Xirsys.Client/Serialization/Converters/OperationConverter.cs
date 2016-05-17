using System;
using Newtonsoft.Json;
using Xirsys.Client.Models.WebSocket;

namespace Xirsys.Client.Serialization.Converters
{
    public class OperationConverter : JsonConverter
    {
        public const String ON_PEERS_OPER = "peers";
        public const String PEER_CONN_OPER = "peer_connected";
        public const String PEER_REM_OPER = "peer_removed";
        public const String MSG_OPER = "message";
        public const String SESSION_OPER = "session";

        public override void WriteJson(JsonWriter writer, Object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            // bleh unboxing
            var enumValue = (OperationType)value;
            switch (enumValue)
            {
                case OperationType.Message:
                    writer.WriteValue(MSG_OPER);
                    break;
                case OperationType.PeerConnected:
                    writer.WriteValue(PEER_CONN_OPER);
                    break;
                case OperationType.PeerRemoved:
                    writer.WriteValue(PEER_REM_OPER);
                    break;
                case OperationType.OnPeers:
                    writer.WriteValue(ON_PEERS_OPER);
                    break;
                case OperationType.Session:
                    writer.WriteValue(SESSION_OPER);
                    break;
                default:
                    writer.WriteNull();
                    break;
            }
        }

        public override Object ReadJson(JsonReader reader, Type objectType, Object existingValue, JsonSerializer serializer)
        {
            String enumStrValue = (String)reader.Value;
            switch (enumStrValue)
            {
                case MSG_OPER:
                    return OperationType.Message;
                case PEER_CONN_OPER:
                    return OperationType.PeerConnected;
                case PEER_REM_OPER:
                    return OperationType.PeerRemoved;
                case ON_PEERS_OPER:
                    return OperationType.OnPeers;
                case SESSION_OPER:
                    return OperationType.Session;
                default:
                    return OperationType.Unknown;
            }
        }

        public override Boolean CanConvert(Type objectType)
        {
            return objectType == typeof(String);
        }
    }
}

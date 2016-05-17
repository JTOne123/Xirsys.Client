using System.Runtime.Serialization;
using Newtonsoft.Json;
using Xirsys.Client.Serialization.Converters;

namespace Xirsys.Client.Models.WebSocket.Payloads
{
    [DataContract]
    public abstract class BaseSessionPayload
    {
        [DataMember]
        [JsonConverter(typeof(SessionPayloadTypeConverter))]
        public SessionPayloadType Type { get; set; }
    }
}

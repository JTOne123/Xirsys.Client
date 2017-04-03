using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xirsys.Client.Models.WebSocket;
using Xirsys.Client.Serialization.Converters;

namespace Xirsys.Client.Serialization
{
    public static class JsonNetExtensions
    {
        private static readonly JsonSerializer s_SessionPayloadTypeSerializer;

        static JsonNetExtensions()
        {
            s_SessionPayloadTypeSerializer = JsonSerializer.CreateDefault();
            s_SessionPayloadTypeSerializer.Converters.Add(new SessionPayloadTypeConverter());
        }

        public static TPayload AsPayload<TPayload>(this BaseWireModel wireModel)
        {
            return AsPayload<TPayload>(wireModel, default(TPayload));
        }

        public static TPayload AsPayload<TPayload>(this BaseWireModel wireModel, TPayload defaultValue)
        {
            var payloadJObject = wireModel.Payload as JObject;
            if (payloadJObject == null)
            {
                return defaultValue;
            }

            return payloadJObject.ToObject<TPayload>();
        }

        public static SessionPayloadType GetSessionPayloadType(this BaseWireModel wireModel)
        {
            var payloadJObject = wireModel.Payload as JObject;
            if (payloadJObject == null)
            {
                return SessionPayloadType.Unknown;
            }

            var sessionType = payloadJObject["type"];
            if (sessionType == null ||
                sessionType.Type != JTokenType.String)
            {
                return SessionPayloadType.Unknown;
            }

            return sessionType.ToObject<SessionPayloadType>(s_SessionPayloadTypeSerializer);
        }
    }
}

using System;
using Newtonsoft.Json;
using Xirsys.Client.Models.WebSocket;

namespace Xirsys.Client.Serialization.Converters
{
    public class SessionPayloadTypeConverter : JsonConverter
    {
        public const String ICE_SESSION_TYPE = "ice";
        public const String OFFER_SESSION_TYPE = "offer";
        public const String CANDIDATE_SESSION_TYPE = "candidate";


        public override void WriteJson(JsonWriter writer, Object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            // bleh unboxing
            var enumValue = (SessionPayloadType)value;
            switch (enumValue)
            {
                case SessionPayloadType.Ice:
                    writer.WriteValue(ICE_SESSION_TYPE);
                    break;
                case SessionPayloadType.Offer:
                    writer.WriteValue(OFFER_SESSION_TYPE);
                    break;
                case SessionPayloadType.Candidate:
                    writer.WriteValue(CANDIDATE_SESSION_TYPE);
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
                case ICE_SESSION_TYPE:
                    return SessionPayloadType.Ice;
                case OFFER_SESSION_TYPE:
                    return SessionPayloadType.Offer;
                case CANDIDATE_SESSION_TYPE:
                    return SessionPayloadType.Candidate;
                default:
                    return SessionPayloadType.Unknown;
            }
        }

        public override Boolean CanConvert(Type objectType)
        {
            return objectType == typeof(String);
        }
    }
}

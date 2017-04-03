using System;
using Newtonsoft.Json;
using Xirsys.Client.Models.REST;

namespace Xirsys.Client.Serialization.Converters
{
    public class StatMeasurementConverter : JsonConverter
    {
        public const String ROUTER_SUB_MEASURE = "router:sub";
        public const String ROUTER_PKT_MEASURE = "router:pkt";
        public const String TURN_SESION_MEASURE = "turn:sess";
        public const String TURN_DATA_MEASURE = "turn:data";
        public const String STUN_SESSION_MEASURE = "stun:sess";
        public const String STUN_DATA_MEASURE = "stun:data";
        public const String API3_MEASURE = "api:3";

        public override void WriteJson(JsonWriter writer, Object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            // bleh unboxing
            var enumValue = (StatMeasurement)value;
            switch (enumValue)
            {
                case StatMeasurement.RouterSubscriptions:
                    writer.WriteValue(ROUTER_SUB_MEASURE);
                    break;
                case StatMeasurement.RouterPackets:
                    writer.WriteValue(ROUTER_PKT_MEASURE);
                    break;
                case StatMeasurement.TurnSessions:
                    writer.WriteValue(TURN_SESION_MEASURE);
                    break;
                case StatMeasurement.TurnData:
                    writer.WriteValue(TURN_DATA_MEASURE);
                    break;
                case StatMeasurement.StunSessions:
                    writer.WriteValue(STUN_SESSION_MEASURE);
                    break;
                case StatMeasurement.StunData:
                    writer.WriteValue(STUN_DATA_MEASURE);
                    break;
                case StatMeasurement.ApiCalls:
                    writer.WriteValue(API3_MEASURE);
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
                case ROUTER_SUB_MEASURE:
                    return StatMeasurement.RouterSubscriptions;
                case ROUTER_PKT_MEASURE:
                    return StatMeasurement.RouterPackets;
                case TURN_SESION_MEASURE:
                    return StatMeasurement.TurnSessions;
                case TURN_DATA_MEASURE:
                    return StatMeasurement.TurnData;
                case STUN_SESSION_MEASURE:
                    return StatMeasurement.StunSessions;
                case STUN_DATA_MEASURE:
                    return StatMeasurement.StunData;
                case API3_MEASURE:
                    return StatMeasurement.ApiCalls;
                default:
                    return StatMeasurement.Unknown;
            }
        }

        public override Boolean CanConvert(Type objectType)
        {
            return objectType == typeof(String);
        }
    }
}

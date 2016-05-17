using System;
using Newtonsoft.Json;
using Xirsys.Client.Models.REST;

namespace Xirsys.Client.Serialization.Converters
{
    public class ApiHttpVerbConverter : JsonConverter
    {
        public const String GET_VERB = "GET";
        public const String POST_VERB = "POST";
        public const String PUT_VERB = "PUT";
        public const String DELETE_VERB = "DELETE";

        public override void WriteJson(JsonWriter writer, Object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            // bleh unboxing
            var enumValue = (ApiHttpVerb)value;
            switch (enumValue)
            {
                case ApiHttpVerb.Get:
                    writer.WriteValue(GET_VERB);
                    break;
                case ApiHttpVerb.Post:
                    writer.WriteValue(POST_VERB);
                    break;
                case ApiHttpVerb.Put:
                    writer.WriteValue(PUT_VERB);
                    break;
                case ApiHttpVerb.Delete:
                    writer.WriteValue(DELETE_VERB);
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
                case GET_VERB:
                    return ApiHttpVerb.Get;
                case POST_VERB:
                    return ApiHttpVerb.Post;
                case PUT_VERB:
                    return ApiHttpVerb.Put;
                case DELETE_VERB:
                    return ApiHttpVerb.Delete;
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

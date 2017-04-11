using System;
using Newtonsoft.Json;
using Xirsys.Client.Models.REST;

namespace Xirsys.Client.Serialization.Converters
{
    public class DiscoveryActionConverter : JsonConverter
    {
        public const String LAYERS_ACTION = "layers";
        public const String PATHS_ACTION = "paths";

        public override void WriteJson(JsonWriter writer, Object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            // bleh unboxing
            var enumValue = (DiscoveryAction)value;
            switch (enumValue)
            {
                case DiscoveryAction.Layers:
                    writer.WriteValue(LAYERS_ACTION);
                    break;
                case DiscoveryAction.Paths:
                    writer.WriteValue(PATHS_ACTION);
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
                case LAYERS_ACTION:
                    return DiscoveryAction.Layers;
                case PATHS_ACTION:
                    return DiscoveryAction.Paths;
                default:
                    return DiscoveryAction.Unknown;
            }
        }

        public override Boolean CanConvert(Type objectType)
        {
            return objectType == typeof(String);
        }
    }
}

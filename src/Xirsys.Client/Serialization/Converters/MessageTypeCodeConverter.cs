using System;
using Newtonsoft.Json;
using Xirsys.Client.Models.WebSocket;

namespace Xirsys.Client.Serialization.Converters
{
    public class MessageTypeCodeConverter : JsonConverter
    {
        public const String USER_TYPE_CODE = "u";

        public override void WriteJson(JsonWriter writer, Object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var enumValue = (MessageTypeCode)value;
            switch(enumValue)
            {
                case MessageTypeCode.User:
                    writer.WriteValue(USER_TYPE_CODE);
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
                case USER_TYPE_CODE:
                    return MessageTypeCode.User;
                default:
                    return MessageTypeCode.Unknown;
            }
        }

        public override Boolean CanConvert(Type objectType)
        {
            return objectType == typeof(String);
        }
    }
}

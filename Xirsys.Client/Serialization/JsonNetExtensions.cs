using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xirsys.Client.Models.WebSocket;
using Xirsys.Client.Serialization.Converters;

namespace Xirsys.Client.Serialization
{
    public static class JsonNetExtensions
    {
        private static readonly JsonSerializerSettings s_DefaultJsonSerializerSettings;
        private static readonly JsonSerializerSettings s_SerializeNullJsonSerializerSettings;

        private static readonly JsonSerializer s_DefaultJsonSerializer;
        private static readonly JsonSerializer s_SerializeNullJsonSerializer;
        private static readonly JsonSerializer s_SessionPayloadTypeSerializer;

        static JsonNetExtensions()
        {
            s_DefaultJsonSerializerSettings = new JsonSerializerSettings()
                {
                    Formatting = Formatting.None,
                    NullValueHandling = NullValueHandling.Ignore,
                    TypeNameHandling = TypeNameHandling.None,

                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                };
            s_SerializeNullJsonSerializerSettings = new JsonSerializerSettings()
                {
                    Formatting = Formatting.None,
                    NullValueHandling = NullValueHandling.Include,
                    TypeNameHandling = TypeNameHandling.None,

                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                };

            s_DefaultJsonSerializer = JsonSerializer.CreateDefault(s_DefaultJsonSerializerSettings);
            s_SerializeNullJsonSerializer = JsonSerializer.CreateDefault(s_SerializeNullJsonSerializerSettings);

            s_SessionPayloadTypeSerializer = JsonSerializer.CreateDefault(s_DefaultJsonSerializerSettings);
            s_SessionPayloadTypeSerializer.Converters.Add(new SessionPayloadTypeConverter());
        }

        public static String SerializeObject(this Object value, Boolean serializeNull = false)
        {
            if (value == null)
            {
                return serializeNull
                    ? "null"
                    : "";
            }

            var stringWriter = new StringWriter(new StringBuilder(256), CultureInfo.InvariantCulture);
            using (var jsonTextWriter = new JsonTextWriter(stringWriter))
            {
                JsonSerializer serializer = serializeNull
                    ? s_SerializeNullJsonSerializer
                    : s_DefaultJsonSerializer;

                jsonTextWriter.Formatting = serializer.Formatting;
                serializer.Serialize(jsonTextWriter, value, null);
            }
            return stringWriter.ToString();
        }

        public static TObject DeserializeObject<TObject>(this String value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            /*
            if (!jsonSerializer.IsCheckAdditionalContentSet())
                jsonSerializer.CheckAdditionalContent = true;
            */
            using (var jsonTextReader = new JsonTextReader(new StringReader(value)))
            {
                return s_DefaultJsonSerializer.Deserialize<TObject>(jsonTextReader);
            }
        }

        public static JObject DeserializeJObject<TObject>(this TObject obj, Boolean serializeNull = false)
        {
            return JObject.FromObject(obj, serializeNull
                ? s_SerializeNullJsonSerializer
                : s_DefaultJsonSerializer);
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

            return payloadJObject.ToObject<TPayload>(s_DefaultJsonSerializer);
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

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
        private static readonly JsonSerializerSettings s_JsonSerializerSettings;

        private static readonly JsonSerializer s_JsonSerializer;
        private static readonly JsonSerializer s_SessionPayloadTypeSerializer;

        static JsonNetExtensions()
        {
            s_JsonSerializerSettings = new JsonSerializerSettings()
                {
                    Formatting = Formatting.None,
                    NullValueHandling = NullValueHandling.Ignore,
                    TypeNameHandling = TypeNameHandling.None,

                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                };

            s_JsonSerializer = JsonSerializer.CreateDefault(s_JsonSerializerSettings);

            s_SessionPayloadTypeSerializer = JsonSerializer.CreateDefault(s_JsonSerializerSettings);
            s_SessionPayloadTypeSerializer.Converters.Add(new SessionPayloadTypeConverter());
        }

        public static String SerializeObject(this Object value)
        {
            var stringWriter = new StringWriter(new StringBuilder(256), CultureInfo.InvariantCulture);
            using (var jsonTextWriter = new JsonTextWriter(stringWriter))
            {
                jsonTextWriter.Formatting = s_JsonSerializer.Formatting;
                s_JsonSerializer.Serialize(jsonTextWriter, value, null);
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
                return s_JsonSerializer.Deserialize<TObject>(jsonTextReader);
            }
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

            return payloadJObject.ToObject<TPayload>(s_JsonSerializer);
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

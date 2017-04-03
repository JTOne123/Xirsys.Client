using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Xirsys.Client.Serialization.Converters;

namespace Xirsys.Client.Models.WebSocket
{
    [DataContract]
    public class BaseWireModel
    {
        [DataMember(Name = "t")]
        [JsonConverter(typeof(MessageTypeCodeConverter))]
        public MessageTypeCode TypeCode { get; set; }

        [DataMember(Name = "p")]
        public Object Payload { get; set; }

        [DataMember(Name = "m")]
        public Meta Meta { get; set; }

        public BaseWireModel()
        {
        }

        public BaseWireModel(MessageTypeCode typeCode, Object payload, Meta meta)
        {
            this.TypeCode = typeCode;
            this.Payload = payload;
            this.Meta = meta;
        }
    }
}

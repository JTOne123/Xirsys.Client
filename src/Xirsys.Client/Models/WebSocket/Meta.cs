using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Xirsys.Client.Serialization.Converters;

namespace Xirsys.Client.Models.WebSocket
{
    [DataContract]
    public class Meta
    {
        [DataMember(Name = "f")]
        public String From { get; set; }

        [DataMember(Name = "t")]
        public String To { get; set; }

        [DataMember(Name = "o")]
        [JsonConverter(typeof(OperationConverter))]
        public OperationType OperationType { get; set; }

        public Meta()
        {
        }

        public Meta(String from, OperationType operationType)
            : this(from, null, operationType)
        {
        }

        public Meta(String from, String to, OperationType operationType)
        {
            this.From = from;
            this.To = to;
            this.OperationType = operationType;
        }
    }
}

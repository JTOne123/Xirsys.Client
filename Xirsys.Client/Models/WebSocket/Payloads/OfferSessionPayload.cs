using System;
using System.Runtime.Serialization;

namespace Xirsys.Client.Models.WebSocket.Payloads
{
    [DataContract]
    public class OfferSessionPayload : BaseSessionPayload
    {
        [DataMember]
        public String Sdp { get; set; }
    }
}

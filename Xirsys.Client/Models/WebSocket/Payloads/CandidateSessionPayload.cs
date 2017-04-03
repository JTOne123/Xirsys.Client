using System;
using System.Runtime.Serialization;

namespace Xirsys.Client.Models.WebSocket.Payloads
{
    [DataContract]
    public class CandidateSessionPayload : BaseSessionPayload
    {
        [DataMember]
        public String Id { get; set; }

        [DataMember]
        public Int32 Label { get; set; }

        [DataMember]
        public String Candidate { get; set; }
    }
}

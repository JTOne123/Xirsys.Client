using System.Collections.Generic;
using System.Runtime.Serialization;
using Xirsys.Client.Models.REST;

namespace Xirsys.Client.Models.WebSocket.Payloads
{
    [DataContract]
    public class IceSessionPayload : BaseSessionPayload
    {
        [DataMember]
        public IceSessionList Ice { get; set; }

        public class IceSessionList
        {
            public List<Server> IceServers { get; set; }
        }
    }
}

using System.Collections.Generic;
using System.Runtime.Serialization;
using Xirsys.Client.Models.REST.Wire;

namespace Xirsys.Client.Models.WebSocket.Payloads
{
    [DataContract]
    public class IceSessionPayload : BaseSessionPayload
    {
        [DataMember]
        public IceSessionList Ice { get; set; }

        [DataContract]
        public class IceSessionList
        {
            [DataMember]
            public List<ServerModel> IceServers { get; set; }
        }
    }
}

using System;

namespace Xirsys.Client.Models.REST
{
    public class IceServerModel
    {
        public NatTraversalProtocol Protocol { get; set; }
        public ServerTransportLayer Transport { get; set; }
        public Boolean IsSecure { get; set; }

        // may be domain or IP
        public String Host { get; set; }
        public Int32 Port { get; set; }

        public String UserName { get; set; }
        public String Credential { get; set; }

        public IceServerModel()
        {
        }

        public IceServerModel(NatTraversalProtocol protocol, ServerTransportLayer transport, Boolean isSecure, String host, Int32 port)
            : this (protocol, transport, isSecure, host, port, null, null)
        {
        }

        public IceServerModel(NatTraversalProtocol protocol, ServerTransportLayer transport, Boolean isSecure, String host, Int32 port, String userName, String credential)
        {
            this.Protocol = protocol;
            this.Transport = transport;
            this.IsSecure = isSecure;

            this.Host = host;
            this.Port = port;

            this.UserName = userName;
            this.Credential = credential;
        }
    }
}

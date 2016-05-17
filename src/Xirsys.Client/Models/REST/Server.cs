using System;
using System.Runtime.Serialization;

namespace Xirsys.Client.Models.REST
{
    [DataContract]
    public class Server
    {
        [DataMember(Name = "username")]
        public String UserName { get; set; }

        [DataMember]
        public String Url { get; set; }

        [DataMember]
        public String Credential { get; set; }
    }
}

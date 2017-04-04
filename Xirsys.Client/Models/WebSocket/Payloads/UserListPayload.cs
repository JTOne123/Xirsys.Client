using System;
using System.Collections.Generic;

namespace Xirsys.Client.Models.WebSocket.Payloads
{
    public class UserListPayload
    {
        public List<String> Users { get; set; }

        public UserListPayload()
        {
            this.Users = new List<String>();
        }

        public UserListPayload(List<String> users)
        {
            this.Users = users ?? new List<String>();
        }
    }
}

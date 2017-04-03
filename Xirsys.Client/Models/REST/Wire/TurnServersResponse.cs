using System;
using System.Collections.Generic;

namespace Xirsys.Client.Models.REST.Wire
{
    public class TurnServersResponse
    {
        public List<ServerModel> IceServers { get; protected set; }

        public TurnServersResponse()
        {
            this.IceServers = new List<ServerModel>();
        }

        public TurnServersResponse(List<ServerModel> iceServers)
        {
            this.IceServers = iceServers ?? new List<ServerModel>();
        }

        public TurnServersResponse(TurnServersResponse other)
            : this(other.IceServers)
        {
        }

        protected Boolean Equals(TurnServersResponse other)
        {
            return Equals(IceServers, other.IceServers);
        }

        public override Boolean Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TurnServersResponse) obj);
        }

        public override Int32 GetHashCode()
        {
            return (IceServers != null ? IceServers.GetHashCode() : 0);
        }

        public static Boolean operator ==(TurnServersResponse left, TurnServersResponse right)
        {
            return Equals(left, right);
        }

        public static Boolean operator !=(TurnServersResponse left, TurnServersResponse right)
        {
            return !Equals(left, right);
        }
    }
}

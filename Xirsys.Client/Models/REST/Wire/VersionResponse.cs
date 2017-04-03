using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Xirsys.Client.Models.REST
{
    [DataContract]
    public class VersionResponse
    {
        public const String VERSION_PROP = "_ver_";

        [DataMember(Name = VERSION_PROP)]
        public String Version { get; set; }

        public VersionResponse()
        {
        }

        public VersionResponse(String version)
        {
            this.Version = version;
        }

        protected Boolean Equals(VersionResponse other)
        {
            return String.Equals(Version, other.Version);
        }

        public override Boolean Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((VersionResponse) obj);
        }

        public override int GetHashCode()
        {
            return (Version != null ? Version.GetHashCode() : 0);
        }

        public static Boolean operator ==(VersionResponse left, VersionResponse right)
        {
            return Equals(left, right);
        }

        public static Boolean operator !=(VersionResponse left, VersionResponse right)
        {
            return !Equals(left, right);
        }
    }
}

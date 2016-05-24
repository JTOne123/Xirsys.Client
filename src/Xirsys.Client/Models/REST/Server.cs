﻿using System;
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

        public Server()
        {
        }

        public Server(String url)
            : this(null, url, null)
        {
        }

        public Server(String userName, String url, String credential)
        {
            this.UserName = userName;
            this.Url = url;
            this.Credential = credential;
        }

        public Server(Server other)
            : this(other.UserName, other.Url, other.Credential)
        {
        }

        protected Boolean Equals(Server other)
        {
            return String.Equals(UserName, other.UserName) && String.Equals(Url, other.Url) && String.Equals(Credential, other.Credential);
        }

        public override Boolean Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Server) obj);
        }

        public override Int32 GetHashCode()
        {
            unchecked
            {
                Int32 hashCode = (UserName != null ? UserName.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Url != null ? Url.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Credential != null ? Credential.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static Boolean operator ==(Server left, Server right)
        {
            return Equals(left, right);
        }

        public static Boolean operator !=(Server left, Server right)
        {
            return !Equals(left, right);
        }
    }
}

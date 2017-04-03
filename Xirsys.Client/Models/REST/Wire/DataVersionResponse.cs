using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Xirsys.Client.Models.REST.Wire
{
    [DataContract]
    public class DataVersionResponse<TData> : VersionResponse
    {
        [DataMember(Name = "data")]
        public TData Data { get; set; }


        public DataVersionResponse()
            : base()
        {
            this.Data = default(TData);
        }

        public DataVersionResponse(TData data, String version)
            : base(version)
        {
            this.Data = data;
        }

        protected Boolean Equals(DataVersionResponse<TData> other)
        {
            return base.Equals(other) && EqualityComparer<TData>.Default.Equals(Data, other.Data);
        }

        public override Boolean Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DataVersionResponse<TData>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ EqualityComparer<TData>.Default.GetHashCode(Data);
            }
        }

        public static Boolean operator ==(DataVersionResponse<TData> left, DataVersionResponse<TData> right)
        {
            return Equals(left, right);
        }

        public static Boolean operator !=(DataVersionResponse<TData> left, DataVersionResponse<TData> right)
        {
            return !Equals(left, right);
        }
    }
}

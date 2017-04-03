using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Xirsys.Client.Models.REST.Wire
{
    [DataContract]
    public class SimpleDataWrapper<TData>
    {
        [DataMember(Name = "value")]
        public TData Value { get; set; }

        public SimpleDataWrapper()
        {
            this.Value = default(TData);
        }

        public SimpleDataWrapper(TData value)
        {
            this.Value = value;
        }

        protected Boolean Equals(SimpleDataWrapper<TData> other)
        {
            return EqualityComparer<TData>.Default.Equals(Value, other.Value);
        }

        public override Boolean Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SimpleDataWrapper<TData>) obj);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<TData>.Default.GetHashCode(Value);
        }

        public static Boolean operator ==(SimpleDataWrapper<TData> left, SimpleDataWrapper<TData> right)
        {
            return Equals(left, right);
        }

        public static Boolean operator !=(SimpleDataWrapper<TData> left, SimpleDataWrapper<TData> right)
        {
            return !Equals(left, right);
        }
    }
}

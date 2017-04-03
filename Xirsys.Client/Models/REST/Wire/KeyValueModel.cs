using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Xirsys.Client.Models.REST.Wire
{
    [DataContract]
    public class KeyValueModel<TData>
    {
        public const String KEY_PROP = "k";
        public const String VALUE_PROP = "v";

        [DataMember(Name = KEY_PROP)]
        public String Key { get; set; }

        [DataMember(Name = VALUE_PROP)]
        public TData Value { get; set; }

        public KeyValueModel()
            : this(null, default(TData))
        {
        }

        public KeyValueModel(String key, TData value)
        {
            this.Key = key;
            this.Value = value;
        }

        protected Boolean Equals(KeyValueModel<TData> other)
        {
            return String.Equals(Key, other.Key) && EqualityComparer<TData>.Default.Equals(Value, other.Value);
        }

        public override Boolean Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((KeyValueModel<TData>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Key != null ? Key.GetHashCode() : 0) * 397) ^ EqualityComparer<TData>.Default.GetHashCode(Value);
            }
        }

        public static Boolean operator ==(KeyValueModel<TData> left, KeyValueModel<TData> right)
        {
            return Equals(left, right);
        }

        public static Boolean operator !=(KeyValueModel<TData> left, KeyValueModel<TData> right)
        {
            return !Equals(left, right);
        }
    }
}

using System;
using System.Collections.Generic;

namespace Xirsys.Client.Models.REST
{
    public class TimeSeriesDataKey<TData>
    {
        public DateTime Timestamp { get; set; }
        public String Context { get; set; }
        public TData Data { get; set; }

        public TimeSeriesDataKey()
        {
        }

        public TimeSeriesDataKey(DateTime timestamp, String context, TData data)
        {
            this.Timestamp = timestamp;
            this.Context = context;
            this.Data = data;
        }

        public TimeSeriesDataKey(TimeSeriesDataKey<TData> other)
            : this(other.Timestamp, other.Context, other.Data)
        {
        }

        protected Boolean Equals(TimeSeriesDataKey<TData> other)
        {
            return Timestamp.Equals(other.Timestamp) && String.Equals(Context, other.Context) && EqualityComparer<TData>.Default.Equals(Data, other.Data);
        }

        public override Boolean Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TimeSeriesDataKey<TData>) obj);
        }

        public override Int32 GetHashCode()
        {
            unchecked
            {
                Int32 hashCode = Timestamp.GetHashCode();
                hashCode = (hashCode*397) ^ (Context != null ? Context.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ EqualityComparer<TData>.Default.GetHashCode(Data);
                return hashCode;
            }
        }

        public static Boolean operator ==(TimeSeriesDataKey<TData> left, TimeSeriesDataKey<TData> right)
        {
            return Equals(left, right);
        }

        public static Boolean operator !=(TimeSeriesDataKey<TData> left, TimeSeriesDataKey<TData> right)
        {
            return !Equals(left, right);
        }
    }
}

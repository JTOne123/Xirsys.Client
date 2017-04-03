using System;
using System.Runtime.Serialization;

namespace Xirsys.Client.Models.REST
{
    [DataContract]
    public class StatsResponse
    {
        [DataMember(Name = "sum")]
        public Int64 Sum { get; set; }

        [DataMember(Name = "min")]
        public Int64 Min { get; set; }

        [DataMember(Name = "max")]
        public Int64 Max { get; set; }

        [DataMember(Name = "count")]
        public Int64 Count { get; set; }

        public StatsResponse()
        {
        }

        public StatsResponse(Int64 sum, Int64 min, Int64 max, Int64 count)
        {
            this.Sum = sum;
            this.Min = min;
            this.Max = max;
            this.Count = count;
        }

        public StatsResponse(StatsResponse other)
            : this(other.Sum, other.Min, other.Max, other.Count)
        {
        }

        protected Boolean Equals(StatsResponse other)
        {
            return Sum == other.Sum && Min == other.Min && Max == other.Max && Count == other.Count;
        }

        public override Boolean Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((StatsResponse) obj);
        }

        public override Int32 GetHashCode()
        {
            unchecked
            {
                Int32 hashCode = Sum.GetHashCode();
                hashCode = (hashCode*397) ^ Min.GetHashCode();
                hashCode = (hashCode*397) ^ Max.GetHashCode();
                hashCode = (hashCode*397) ^ Count.GetHashCode();
                return hashCode;
            }
        }

        public static Boolean operator ==(StatsResponse left, StatsResponse right)
        {
            return Equals(left, right);
        }

        public static Boolean operator !=(StatsResponse left, StatsResponse right)
        {
            return !Equals(left, right);
        }

        public static StatsResponse operator +(StatsResponse left, StatsResponse right)
        {
            return new StatsResponse(
                left.Sum + right.Sum,
                left.Min + right.Min,
                left.Max + right.Max,
                left.Count + right.Count);
        }
    }
}

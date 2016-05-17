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

        public static StatsResponse operator +(StatsResponse m1, StatsResponse m2)
        {
            return new StatsResponse(
                m1.Sum   + m2.Sum,
                m1.Min   + m2.Min,
                m1.Max   + m2.Max,
                m1.Count + m2.Count);
        }

        public StatsResponse()
        {
        }

        public StatsResponse(long sum, long min, long max, long count)
        {
            this.Sum = sum;
            this.Min = min;
            this.Max = max;
            this.Count = count;
        }

        public StatsResponse(StatsResponse other)
        {
            this.Sum = other.Sum;
            this.Min = other.Min;
            this.Max = other.Max;
            this.Count = other.Count;
        }
    }
}

using System;

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
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xirsys.Client.Models.REST;
using Xirsys.Client.Serialization;
using Xirsys.Client.Utilities;

namespace Xirsys.Client
{
    public partial class XirsysApiClient
    {
        protected const String DATA_SERVICE = "_data";

        // currently broken if value is a List/Enumerable type of object
        public Task<XirsysResponseModel<NullData>> AddDataKeyAsync<TData>(String path, String key, TData value)
        {
            return InternalPostAsync<Object, NullData>(GetServiceMethodPath(DATA_SERVICE, path), 
                new { k = key, v = value });
        }

        public Task<XirsysResponseModel<Int32>> RemoveDataKeyAsync(String path, String key)
        {
            return InternalDeleteAsync<Int32>(GetServiceMethodPath(DATA_SERVICE, path),
                new KeyValueList<String, String>(1)
                    {
                        { "k", key }
                    });
        }


        public Task<XirsysResponseModel<Int32>> RemoveAllDataKeysAsync(String path)
        {
            return InternalDeleteAsync<Int32>(GetServiceMethodPath(DATA_SERVICE, path));
        }

        public Task<XirsysResponseModel<TData>> GetDataKeyAsync<TData>(String path, String key)
        {
            return InternalGetAsync<TData>(GetServiceMethodPath(DATA_SERVICE, path),
                new KeyValueList<String, String>(1)
                    {
                        { "k", key }
                    });
        }

        public Task<XirsysResponseModel<List<String>>> ListDataKeysAsync(String path)
        {
            return InternalGetAsync<List<String>>(GetServiceMethodPath(DATA_SERVICE, path));
        }

        public async Task<XirsysResponseModel<List<TimeSeriesDataKey<Object>>>> GetDataKeyTimeSeriesAsync(String path, String key)
        {
            var apiResponse = await InternalGetAsync<List<List<Object>>>(GetServiceMethodPath(DATA_SERVICE, path),
                new KeyValueList<String, String>(2)
                    {
                        { "k", key },
                        { "time_series", "1" },
                    })
                    .ConfigureAwait(false);

            return new XirsysResponseModel<List<TimeSeriesDataKey<Object>>>(
                apiResponse.Status, 
                apiResponse.ErrorResponse,
                ConvertResult(apiResponse.Data));
        }

        public async Task<XirsysResponseModel<List<TimeSeriesDataKey<Object>>>> GetDataKeyTimeSeriesAsync(String path, String key, DatePrecision groupPrecision, DateTime groupStart, Nullable<DateTime> groupEnd)
        {
            var dateTimeStrFormat = groupPrecision.GetDateTimeFormatExact();
            var parameters = new KeyValueList<String, String>(4)
                    {
                        { "k", key },
                        { "time_series", "1" },
                        { "gs", groupStart.ToString(dateTimeStrFormat) },
                    };
            if (groupEnd.HasValue)
            {
                // if groupEnd is not specified the end date becomes the last DateTime within groupPrecision
                parameters.Add("ge", groupEnd.Value.ToString(dateTimeStrFormat));
            }
            var apiResponse = await InternalGetAsync<List<List<Object>>>(GetServiceMethodPath(DATA_SERVICE, path), parameters)
                .ConfigureAwait(false);

            return new XirsysResponseModel<List<TimeSeriesDataKey<Object>>>(
                apiResponse.Status,
                apiResponse.ErrorResponse,
                ConvertResult(apiResponse.Data));
        }

        private static List<TimeSeriesDataKey<Object>> ConvertResult(List<List<Object>> listOfLists)
        {
            if (listOfLists == null)
            {
                return new List<TimeSeriesDataKey<Object>>();
            }

            var timeSeriesData = new List<TimeSeriesDataKey<Object>>(listOfLists.Count);
            foreach (var innerList in listOfLists)
            {
                // returns a weird inner array which should have 3 items
                // first item is unix timestamp (when this value was inserted)
                // second item is the context/ident for this account/value?
                // third item is the value itself
                if (innerList.Count < 3)
                {
                    // skipping lists with not enough values
                    continue;
                }
                // ignore more than 3 items

                timeSeriesData.Add(new TimeSeriesDataKey<Object>(
                    ((Int64)innerList[0]).GetDateTimeFromUnix(),
                    (String)innerList[1],
                    innerList[2]));
            }

            return timeSeriesData;
        }
    }
}

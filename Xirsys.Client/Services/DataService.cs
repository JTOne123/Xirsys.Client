using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xirsys.Client.Models.REST;
using Xirsys.Client.Models.REST.Wire;
using Xirsys.Client.Serialization;
using Xirsys.Client.Utilities;

namespace Xirsys.Client
{
    public partial class XirsysApiClient
    {
        protected const String DATA_SERVICE = "_data";

        public Task<XirsysResponseModel<DataVersionResponse<TData>>> AddDataKeyAsync<TData>(String path, String key, TData value, 
            CancellationToken cancelToken = default(CancellationToken))
        {
            return AddDataKeyAsync(path, key, value, null,
                cancelToken: cancelToken);
        }

        public Task<XirsysResponseModel<DataVersionResponse<TData>>> AddDataKeyAsync<TData>(String path, String key, TData value, String oldVersion, 
            CancellationToken cancelToken = default(CancellationToken))
        {
            KeyValueModel<Object> addDataObj;
            Func<KeyValueModel<Object>, String> serializeDataFunc;
            Func<String, JObject, XirsysResponseModel<DataVersionResponse<TData>>> okParseFunc;

            if (oldVersion != null)
            {
                serializeDataFunc = (contentData) =>
                    {
                        var intermediateObj = JObject.FromObject(contentData);
                        var valueToken = intermediateObj[KeyValueModel<Object>.VALUE_PROP];
                        if (valueToken == null || valueToken.Type != JTokenType.Object)
                        {
                            Logger.LogWarning("Failed to locate {0} property in Content model. Cannot insert {1} property.", KeyValueModel<Object>.VALUE_PROP, VersionResponse.VERSION_PROP);
                        }
                        else
                        {
                            valueToken[VersionResponse.VERSION_PROP] = oldVersion;
                        }
                        return intermediateObj.ToString(Formatting.None);
                    };
            }
            else
            {
                serializeDataFunc = null;
            }

            if (value != null && value.GetType().IsSimpleType())
            {
                addDataObj = new KeyValueModel<Object>(key, new SimpleDataWrapper<TData>(value));
                okParseFunc = SimpleDataParseResponseWithVersion<TData>;
            }
            else
            {
                addDataObj = new KeyValueModel<Object>(key, value);
                okParseFunc = DataParseResponseWithVersion<TData>;
            }

            return InternalPutAsync(GetServiceMethodPath(DATA_SERVICE, path), addDataObj, serializeContentData: serializeDataFunc, okParseResponse: okParseFunc,
                cancelToken: cancelToken);
        }


        public Task<XirsysResponseModel<Int32>> RemoveDataKeyAsync(String path, String key, 
            CancellationToken cancelToken = default(CancellationToken))
        {
            return InternalDeleteAsync<Int32>(GetServiceMethodPath(DATA_SERVICE, path),
                new QueryStringList(1)
                    {
                        { "k", key }
                    },
                cancelToken: cancelToken);
        }


        public Task<XirsysResponseModel<Int32>> RemoveAllDataKeysAsync(String path, 
            CancellationToken cancelToken = default(CancellationToken))
        {
            return InternalDeleteAsync<Int32>(GetServiceMethodPath(DATA_SERVICE, path),
                cancelToken: cancelToken);
        }

        public Task<XirsysResponseModel<DataVersionResponse<TData>>> GetDataKeyAsync<TData>(String path, String key, 
            CancellationToken cancelToken = default(CancellationToken))
        {
            Func<String, JObject, XirsysResponseModel<DataVersionResponse<TData>>> okSerializeFunc;
            var qs = new QueryStringList(1)
                {
                    {"k", key}
                };

            if (typeof(TData).IsSimpleType())
            {
                okSerializeFunc = SimpleDataParseResponseWithVersion<TData>;
            }
            else
            {
                okSerializeFunc = DataParseResponseWithVersion<TData>;
            }
            return InternalGetAsync(GetServiceMethodPath(DATA_SERVICE, path), qs, okSerializeFunc,
                cancelToken: cancelToken);
        }

        public Task<XirsysResponseModel<List<String>>> ListDataKeysAsync(String path, 
            CancellationToken cancelToken = default(CancellationToken))
        {
            return InternalGetAsync<List<String>>(GetServiceMethodPath(DATA_SERVICE, path),
                cancelToken: cancelToken);
        }

        public async Task<XirsysResponseModel<List<TimeSeriesDataKey<TData>>>> GetDataKeyTimeSeriesAsync<TData>(String path, String key, 
            CancellationToken cancelToken = default(CancellationToken))
        {
            var apiResponse = await InternalGetAsync<List<List<JToken>>>(GetServiceMethodPath(DATA_SERVICE, path),
                new QueryStringList(2)
                    {
                        { "k", key },
                        { "time_series", "1" },
                    },
                cancelToken: cancelToken);

            return new XirsysResponseModel<List<TimeSeriesDataKey<TData>>>(
                apiResponse.Status, 
                apiResponse.ErrorResponse,
                ConvertResult<TData>(apiResponse.Data),
                apiResponse.RawHttpResponse);
        }

        public async Task<XirsysResponseModel<List<TimeSeriesDataKey<TData>>>> GetDataKeyTimeSeriesAsync<TData>(String path, String key, DatePrecision groupPrecision, DateTime groupStart, Nullable<DateTime> groupEnd, 
            CancellationToken cancelToken = default(CancellationToken))
        {
            var dateTimeStrFormat = groupPrecision.GetDateTimeFormatExact();
            var parameters = new QueryStringList(4)
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
            var apiResponse = await InternalGetAsync<List<List<JToken>>>(GetServiceMethodPath(DATA_SERVICE, path), parameters,
                cancelToken: cancelToken);

            return new XirsysResponseModel<List<TimeSeriesDataKey<TData>>>(
                apiResponse.Status,
                apiResponse.ErrorResponse,
                ConvertResult<TData>(apiResponse.Data),
                apiResponse.RawHttpResponse);
        }

        private static List<TimeSeriesDataKey<TResponseData>> ConvertResult<TResponseData>(List<List<JToken>> listOfLists)
        {
            if (listOfLists == null)
            {
                return new List<TimeSeriesDataKey<TResponseData>>();
            }

            var timeSeriesData = new List<TimeSeriesDataKey<TResponseData>>(listOfLists.Count);
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

                if (!(innerList[0] is JValue) ||
                    !(innerList[1] is JValue) ||
                    !(innerList[2] is JObject))
                {
                    // one of the types is invalid
                    continue;
                }

                timeSeriesData.Add(new TimeSeriesDataKey<TResponseData>(
                    innerList[0].ToObject<Int64>().GetDateTimeFromUnix(),
                    innerList[1].ToObject<String>(),
                    innerList[2].ToObject<TResponseData>()));
            }

            return timeSeriesData;
        }

        protected XirsysResponseModel<DataVersionResponse<TResponseData>> DataParseResponseWithVersion<TResponseData>(String responseStr, JObject parsedJObject)
        {
            return DataParseResponseWithVersion<TResponseData, TResponseData>(responseStr, parsedJObject, (deserialized) => deserialized);
        }

        protected XirsysResponseModel<DataVersionResponse<TResponseData>> SimpleDataParseResponseWithVersion<TResponseData>(String responseStr, JObject parsedJObject)
        {
            return DataParseResponseWithVersion<TResponseData, SimpleDataWrapper<TResponseData>>(responseStr, parsedJObject, (deserialized) => deserialized.Value);
        }

        protected XirsysResponseModel<DataVersionResponse<TResponseData>> DataParseResponseWithVersion<TResponseData, TSerializedData>(String responseStr, JObject parsedJObject,
            Func<TSerializedData, TResponseData> serializedToResponseFunc = null)
        {
            var valueToken = parsedJObject[VALUE_PROP];
            if (valueToken == null ||
                valueToken.Type != JTokenType.Object)
            {
                // value should never be null or NOT an object, if it is the service layer has some bugs
                Logger.LogWarning("Invalid Xirsys Api Response. Value property is null or not an object. Response: {0}", responseStr);
                return new XirsysResponseModel<DataVersionResponse<TResponseData>>(SystemMessages.ERROR_STATUS, ErrorMessages.Parsing, null, responseStr);
            }

            var valueData = valueToken.ToObject<TSerializedData>();
            if (valueData == null)
            {
                // likewise if we can't serialize back to data type, there is a problem
                Logger.LogWarning("Invalid Xirsys Api Response. Value property did not deserialize to {0}. Response: {1}", typeof(TSerializedData).Name, responseStr);
                return new XirsysResponseModel<DataVersionResponse<TResponseData>>(SystemMessages.ERROR_STATUS, ErrorMessages.Parsing, null, responseStr);
            }

            var versionToken = valueToken[VersionResponse.VERSION_PROP];
            String versionValue;
            if (versionToken != null &&
                versionToken.Type == JTokenType.String)
            {
                versionValue = versionToken.ToObject<String>();
                if (String.IsNullOrEmpty(versionValue))
                {
                    // sort of a problem, but we can continue
                    Logger.LogWarning($"Invalid Xirsys Api Response. {VersionResponse.VERSION_PROP} property was empty. Response: {{1}}", responseStr);
                    versionValue = String.Empty;
                }
            }
            else
            {
                // sort of a problem, but we can continue
                Logger.LogWarning($"Invalid Xirsys Api Response. {VersionResponse.VERSION_PROP} property was not present or invalid type. Response: {{1}}", responseStr);
                versionValue = String.Empty;
            }

            var responseWithVersion = new DataVersionResponse<TResponseData>(serializedToResponseFunc(valueData), versionValue);
            return new XirsysResponseModel<DataVersionResponse<TResponseData>>(SystemMessages.OK_STATUS, responseWithVersion, responseStr);
        }
    }
}

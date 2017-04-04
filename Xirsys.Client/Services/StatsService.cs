using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xirsys.Client.Models.REST;
using Xirsys.Client.Serialization;
using Xirsys.Client.Utilities;

namespace Xirsys.Client
{
    public partial class XirsysApiClient
    {
        protected const String STATS_SERVICE = "_stats";

        public Task<XirsysResponseModel<Object>> AddStatsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<XirsysResponseModel<Object>> RemoveStatsAsync()
        {
            throw new NotImplementedException();
        }

        // not working?
        public async Task<XirsysResponseModel<StatsResponse>> GetStatsAsync(String path, StatMeasurement measurement, 
            DatePrecision groupPrecision, DateTime groupStart, Nullable<DateTime> groupEnd = null)
        {
            ValidateMeasurement(measurement);
            var dateTimeStrFormat = groupPrecision.GetDateTimeFormatExact();

            var parameters = new QueryStringList(3)
                {
                    { "l", measurement.ToStringValue() },
                    { "gs", groupStart.ToString(dateTimeStrFormat) },
                };
            if (groupEnd != null)
            {
                // if groupEnd is not specified the end date becomes the last DateTime within groupPrecision
                parameters.Add("ge", groupEnd.Value.ToString(dateTimeStrFormat));
            }
            return await InternalGetAsync<StatsResponse>(GetServiceMethodPath(STATS_SERVICE, path), parameters, StatParseResponseFix<StatsResponse>)
                .ConfigureAwait(false);
        }

        // not working?
        public async Task<XirsysResponseModel<Dictionary<DateTime, StatsResponse>>> GetStatsBreakdownAsync(String path, StatMeasurement measurement, 
            DatePrecision groupPrecision, DateTime groupStart, Nullable<DateTime> groupEnd = null)
        {
            ValidateMeasurement(measurement);
            var dateTimeStrFormat = groupPrecision.GetDateTimeFormatExact();

            var parameters = new QueryStringList(4)
                {
                    { "l", measurement.ToStringValue() },
                    { "break", "1" },
                    { "gs", groupStart.ToString(dateTimeStrFormat) },
                };
            if (groupEnd != null)
            {
                parameters.Add("ge", groupEnd.Value.ToString(dateTimeStrFormat));
            }
            var statBreakdownResponse = await InternalGetAsync<Dictionary<String, StatsResponse>>(GetServiceMethodPath(STATS_SERVICE, path), parameters, StatParseResponseFix<Dictionary<String, StatsResponse>>)
                .ConfigureAwait(false);

            return new XirsysResponseModel<Dictionary<DateTime, StatsResponse>>(
                statBreakdownResponse.Status,
                statBreakdownResponse.ErrorResponse,
                ConvertResult(statBreakdownResponse.Data, dateTimeStrFormat)
            );
        }

        // also not working?
        public async Task<XirsysResponseModel<StatsResponse>> GetStatsAsync(ApiService apiService, ApiHttpVerb apiHttpVerb,
            DatePrecision groupPrecision, DateTime groupStart, Nullable<DateTime> groupEnd = null, String ident = null)
        {
            ValidateApiService(apiService);
            ValidateApiHttpVerb(apiHttpVerb);
            var dateTimeStrFormat = groupPrecision.GetDateTimeFormatExact();

            var parameters = new QueryStringList(3)
                {
                    { "l", StatMeasurement.ApiCalls.ToStringValue() },
                    { "gs", groupStart.ToString(dateTimeStrFormat) },
                };
            if (groupEnd != null)
            {
                parameters.Add("ge", groupEnd.Value.ToString(dateTimeStrFormat));
            }
            if (!String.IsNullOrEmpty(ident))
            {
                parameters.Add("k", ident);
            }

            return await InternalGetAsync<StatsResponse>(
                GetServiceMethodPath(STATS_SERVICE, $"{apiService.ToStringValue()}/{apiHttpVerb.ToStringValue()}"), 
                parameters, StatParseResponseFix<StatsResponse>)
                .ConfigureAwait(false);
        }

        public async Task<XirsysResponseModel<Dictionary<DateTime, StatsResponse>>> GetStatsBreakdownAsync(ApiService apiService, ApiHttpVerb apiHttpVerb,
            DatePrecision groupPrecision, DateTime groupStart, Nullable<DateTime> groupEnd = null, String ident = null)
        {
            ValidateApiService(apiService);
            ValidateApiHttpVerb(apiHttpVerb);
            var dateTimeStrFormat = groupPrecision.GetDateTimeFormatExact();

            var parameters = new QueryStringList(4)
                {
                    { "l", StatMeasurement.ApiCalls.ToStringValue() },
                    { "break", "1" },
                    { "gs", groupStart.ToString(dateTimeStrFormat) },
                };
            if (groupEnd != null)
            {
                parameters.Add("ge", groupEnd.Value.ToString(dateTimeStrFormat));
            }
            if (!String.IsNullOrEmpty(ident))
            {
                parameters.Add("k", ident);
            }

            var statBreakdownResponse = await InternalGetAsync<Dictionary<String, StatsResponse>>(
                GetServiceMethodPath(STATS_SERVICE, $"{apiService.ToStringValue()}/{apiHttpVerb.ToStringValue()}"),
                parameters, StatParseResponseFix<Dictionary<String, StatsResponse>>)
                .ConfigureAwait(false);

            return new XirsysResponseModel<Dictionary<DateTime, StatsResponse>>(
                statBreakdownResponse.Status,
                statBreakdownResponse.ErrorResponse,
                ConvertResult(statBreakdownResponse.Data, dateTimeStrFormat)
            );
        }

        private static XirsysResponseModel<TResponse> StatParseResponseFix<TResponse>(String responseStr, JObject parsedJObject)
            where TResponse : new()
        {
            // currently the API will return [] instead of an object filled with 0's if there is no data
            // however we want to interpret as our StatResponse object with zero's
            // we'll also handle null here (since this is already on a "success" response, which should mean zero stats)
            var valueToken = parsedJObject[VALUE_PROP];
            if (valueToken == null ||
                (
                    valueToken.Type == JTokenType.Array &&
                    String.Equals("[]", valueToken.ToString())
                ))
            {
                return new XirsysResponseModel<TResponse>(SystemMessages.OK_STATUS, new TResponse());
            }

            // else allow normal JSON.NET deserialization
            return JsonNetExtensions.DeserializeObject<XirsysResponseModel<TResponse>>(responseStr);
        }

        private static void ValidateMeasurement(StatMeasurement measurement)
        {
            switch (measurement)
            {
                case StatMeasurement.RouterSubscriptions:
                case StatMeasurement.RouterPackets:
                case StatMeasurement.TurnSessions:
                case StatMeasurement.TurnData:
                case StatMeasurement.StunSessions:
                case StatMeasurement.StunData:
                    break;
                case StatMeasurement.ApiCalls:
                    throw new ArgumentException("Use overload method for Api stats.", nameof(measurement));
                default:
                    throw new ArgumentException($"Invalid {nameof(StatMeasurement)}", nameof(measurement));
            }
        }

        private static void ValidateApiService(ApiService apiService)
        {
            switch (apiService)
            {
                case ApiService.Accounts:
                case ApiService.Namespace:
                case ApiService.Subscriptions:
                case ApiService.Authorization:
                case ApiService.Stats:
                case ApiService.UserData:
                case ApiService.Host:
                case ApiService.Token:
                    break;
                default:
                    throw new ArgumentException($"Invalid {nameof(ApiService)}", nameof(apiService));
            }
        }

        private static void ValidateApiHttpVerb(ApiHttpVerb apiHttpVerb)
        {
            switch (apiHttpVerb)
            {
                case ApiHttpVerb.Get:
                case ApiHttpVerb.Post:
                case ApiHttpVerb.Put:
                case ApiHttpVerb.Delete:
                    break;
                default:
                    throw new ArgumentException($"Invalid {nameof(ApiHttpVerb)}", nameof(apiHttpVerb));
            }
        }

        private static Dictionary<DateTime, StatsResponse> ConvertResult(Dictionary<String, StatsResponse> fromDict, String dateTimeStrFormat)
        {
            if (fromDict == null)
            {
                return new Dictionary<DateTime, StatsResponse>();
            }

            var toDict = new Dictionary<DateTime, StatsResponse>(fromDict.Count);
            foreach (var fromKvp in fromDict)
            {
                DateTime fromDateTime;
                if (!DateTime.TryParseExact(fromKvp.Key, dateTimeStrFormat, DateTimeFormatInfo.InvariantInfo,
                    DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out fromDateTime))
                {
                    continue;
                }

                if (toDict.ContainsKey(fromDateTime))
                {
                    toDict[fromDateTime] += fromKvp.Value;
                }
                else
                {
                    toDict.Add(fromDateTime, fromKvp.Value);
                }
            }
            return toDict;
        }
    }
}

using System;
using Xirsys.Client.Models.REST;
using Xirsys.Client.Serialization.Converters;

namespace Xirsys.Client.Serialization
{
    public static class SerializeExtensions
    {
        public static String ToStringValue(this StatMeasurement measurement)
        {
            switch (measurement)
            {
                case StatMeasurement.RouterSubscriptions:
                    return StatMeasurementConverter.ROUTER_SUB_MEASURE;
                case StatMeasurement.RouterPackets:
                    return StatMeasurementConverter.ROUTER_PKT_MEASURE;
                case StatMeasurement.TurnSessions:
                    return StatMeasurementConverter.TURN_SESION_MEASURE;
                case StatMeasurement.TurnData:
                    return StatMeasurementConverter.TURN_DATA_MEASURE;
                case StatMeasurement.StunSessions:
                    return StatMeasurementConverter.STUN_SESSION_MEASURE;
                case StatMeasurement.StunData:
                    return StatMeasurementConverter.STUN_DATA_MEASURE;
                case StatMeasurement.ApiCalls:
                    return StatMeasurementConverter.API3_MEASURE;
                case StatMeasurement.Unknown:
                default:
                    return null;
            }
        }

        public static String ToStringValue(this ApiService apiService)
        {
            switch (apiService)
            {
                case ApiService.Accounts:
                    return ApiServiceConverter.ACCOUNTS_SVC;
                case ApiService.Namespace:
                    return ApiServiceConverter.NAMESPACE_SVC;
                case ApiService.Subscriptions:
                    return ApiServiceConverter.SUBSCRIPTION_SVC;
                case ApiService.Authorization:
                    return ApiServiceConverter.AUTHORIZATION_SVC;
                case ApiService.Stats:
                    return ApiServiceConverter.STATS_SVC;
                case ApiService.UserData:
                    return ApiServiceConverter.USER_DATA_SVC;
                case ApiService.Host:
                    return ApiServiceConverter.HOST_SVC;
                case ApiService.Token:
                    return ApiServiceConverter.TOKEN_SVC;
                case ApiService.Unknown:
                default:
                    return null;
            }
        }

        public static String ToStringValue(this ApiHttpVerb apiHttpVerb)
        {
            switch (apiHttpVerb)
            {
                case ApiHttpVerb.Get:
                    return ApiHttpVerbConverter.GET_VERB;
                case ApiHttpVerb.Post:
                    return ApiHttpVerbConverter.POST_VERB;
                case ApiHttpVerb.Put:
                    return ApiHttpVerbConverter.PUT_VERB;
                case ApiHttpVerb.Delete:
                    return ApiHttpVerbConverter.DELETE_VERB;
                case ApiHttpVerb.Unknown:
                default:
                    return null;
            }
        }

        internal static String GetDateTimeFormat(this DatePrecision precision, bool forceExact)
        {
            switch (precision)
            {
                case DatePrecision.Year:
                    return "yyyy";
                case DatePrecision.Month:
                    return "yyyy:M";
                case DatePrecision.Day:
                    return "yyyy:M:d";
                case DatePrecision.Hour:
                    return "yyyy:M:d:H";
                case DatePrecision.Minute:
                    return "yyyy:M:d:H:m";
                default:
                    if (forceExact)
                    {
                        throw new ArgumentException($"Invalid {nameof(DatePrecision)}: {precision}", nameof(precision));
                    }
                    // if forceExact is not true defaults to most precise format (which is currently minute)
                    goto case DatePrecision.Minute;
            }
        }

        public static String GetDateTimeFormat(this DatePrecision precision)
        {
            return GetDateTimeFormat(precision, false);
        }

        public static String GetDateTimeFormatExact(this DatePrecision precision)
        {
            return GetDateTimeFormat(precision, true);
        }
    }
}

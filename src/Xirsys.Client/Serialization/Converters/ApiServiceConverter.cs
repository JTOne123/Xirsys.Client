using System;
using Newtonsoft.Json;
using Xirsys.Client.Models.REST;

namespace Xirsys.Client.Serialization.Converters
{
    public class ApiServiceConverter : JsonConverter
    {
        public const String ACCOUNTS_SVC = "acc";
        public const String NAMESPACE_SVC = "ns";
        public const String SUBSCRIPTION_SVC = "subs";
        public const String AUTHORIZATION_SVC = "auth";
        public const String STATS_SVC = "stats";
        public const String USER_DATA_SVC = "data";
        public const String HOST_SVC = "host";
        public const String TOKEN_SVC = "token";

        public override void WriteJson(JsonWriter writer, Object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            // bleh unboxing
            var enumValue = (ApiService)value;
            switch (enumValue)
            {
                case ApiService.Accounts:
                    writer.WriteValue(ACCOUNTS_SVC);
                    break;
                case ApiService.Namespace:
                    writer.WriteValue(NAMESPACE_SVC);
                    break;
                case ApiService.Subscriptions:
                    writer.WriteValue(SUBSCRIPTION_SVC);
                    break;
                case ApiService.Authorization:
                    writer.WriteValue(AUTHORIZATION_SVC);
                    break;
                case ApiService.Stats:
                    writer.WriteValue(STATS_SVC);
                    break;
                case ApiService.UserData:
                    writer.WriteValue(USER_DATA_SVC);
                    break;
                case ApiService.Host:
                    writer.WriteValue(HOST_SVC);
                    break;
                case ApiService.Token:
                    writer.WriteValue(TOKEN_SVC);
                    break;
                default:
                    writer.WriteNull();
                    break;
            }
        }

        public override Object ReadJson(JsonReader reader, Type objectType, Object existingValue, JsonSerializer serializer)
        {
            String enumStrValue = (String)reader.Value;
            switch (enumStrValue)
            {
                case ACCOUNTS_SVC:
                    return ApiService.Accounts;
                case NAMESPACE_SVC:
                    return ApiService.Namespace;
                case SUBSCRIPTION_SVC:
                    return ApiService.Subscriptions;
                case AUTHORIZATION_SVC:
                    return ApiService.Authorization;
                case STATS_SVC:
                    return ApiService.Stats;
                case USER_DATA_SVC:
                    return ApiService.UserData;
                case HOST_SVC:
                    return ApiService.Host;
                case TOKEN_SVC:
                    return ApiService.Token;
                default:
                    return ApiService.Unknown;
            }
        }

        public override Boolean CanConvert(Type objectType)
        {
            return objectType == typeof(String);
        }
    }
}

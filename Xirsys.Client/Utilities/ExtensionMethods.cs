using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xirsys.Client.Models.REST;

namespace Xirsys.Client.Utilities
{
    public static class ExtensionMethods
    {
        public static bool IsOk<TData>(this XirsysResponseModel<TData> response)
        {
            return String.Equals(response.Status, SystemMessages.OK_STATUS, StringComparison.CurrentCultureIgnoreCase);
        }


        private static readonly Type[] SIMPLE_TYPES = new Type[]
             {
                typeof(Boolean),

                typeof(SByte),
                typeof(Byte),
                typeof(Int16),
                typeof(UInt16),
                typeof(Int32),
                typeof(UInt32),
                typeof(Int64),
                typeof(UInt64),

                typeof(Single),
                typeof(Double),
                typeof(Decimal),

                typeof(Char),
                typeof(String),

                typeof(DateTime),
             };

        public static bool IsSimpleType(this Type type)
        {
            if (type.IsArray ||
                SIMPLE_TYPES.Contains(type))
            {
                return true;
            }

            var typeInfo = type.GetTypeInfo();

            if (typeInfo.IsPrimitive ||
                typeInfo.IsEnum)
            {
                return true;
            }

            if (typeInfo.IsGenericType)
            {
                var genericTypeInfo = typeInfo.GetGenericTypeDefinition();
                if (genericTypeInfo == typeof(Nullable<>))
                {
                    return true;
                }
                if (typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(typeInfo))
                {
                    return true;
                }
            }

            return false;
        }


        private static readonly DateTime EPOCH_DATETIME = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime GetDateTimeFromUnix(this Int32 intUnixTimestamp)
        {
            return EPOCH_DATETIME.AddSeconds(intUnixTimestamp);
        }

        public static DateTime GetDateTimeFromUnix(this Int64 longUnixTimestamp)
        {
            return EPOCH_DATETIME.AddSeconds(longUnixTimestamp);
        }

        public static Double ToUnixTimestamp(this DateTime dateTime)
        {
            return dateTime.ToUniversalTime().Subtract(EPOCH_DATETIME).TotalSeconds;
        }



        public static String ToHttpString(this IEnumerable<KeyValuePair<String, String>> collection)
        {
            if (collection == null)
            {
                return String.Empty;
            }

            // simplified version of what ToString in HttpValueCollection does
            var items = new List<String>(collection.Count());
            foreach (KeyValuePair<String, String> kvp in collection
                // so that duplicate keys are at least serialized into the querystring together
                .OrderBy(x => x.Key))
            {
                String key = kvp.Key;
                String value = kvp.Value;

                if (String.IsNullOrEmpty(key))
                {
                    continue;
                }

                String keyPrefix = Uri.EscapeDataString(key) + "=";
                if (String.IsNullOrEmpty(value))
                {
                    items.Add(keyPrefix);
                }
                else
                {
                    items.Add(String.Concat(keyPrefix, Uri.EscapeDataString(value) ?? String.Empty));
                }
            }

#if LESS_THAN_NET40
            return String.Join("&", items.ToArray());
#else
            return String.Join("&", items);
#endif
        }

        public static List<KeyValuePair<String, String>> ParseQueryString(this String queryString)
        {
            if (queryString == null)
            {
                return new QueryStringList(0);
            }
            if (queryString.Length == 0 || (queryString.Length == 1 && queryString[0] == '?'))
            {
                return new QueryStringList(0);
            }

            // chomp off querystring init
            if (queryString[0] == '?')
            {
                queryString = queryString.Substring(1);
            }

            // parse into our simple KeyValueList implementation
            var queryList = new QueryStringList();
            ParseQueryString(queryString, queryList);
            return queryList;
        }

        static void ParseQueryString(String queryString, List<KeyValuePair<String, String>> queryList)
        {
            if (queryString.Length == 0)
            {
                return;
            }

            int qsLength = queryString.Length;
            // position of current key value pair (with =)
            // between their concat character ampersand
            int pairPos = 0;
            // loop until end of querystring
            while (pairPos <= qsLength)
            {
                // indicate start index of value str
                int valuePos = -1, 
                // indicates end index of value str
                    valueEnd = -1;
                // loop through our queryString until we find next pair between ampersand
                // or end of string
                for (int pos = pairPos; pos < qsLength; pos++)
                {
                    if (valuePos == -1 && queryString[pos] == '=')
                    {
                        valuePos = pos + 1;
                    }
                    else if (queryString[pos] == '&')
                    {
                        valueEnd = pos;
                        break;
                    }
                }

                string name, 
                       value;
                if (valuePos == -1)
                {
                    // no value present
                    name = null;
                    valuePos = pairPos;
                }
                else
                {
                    // unescape string
                    name = Uri.UnescapeDataString(queryString.Substring(pairPos, valuePos - pairPos - 1));
                }
                if (valueEnd < 0)
                {
                    // we are at end of string, set pairPos -1 to signal break
                    pairPos = -1;
                    // valueEnd is simply end of qs
                    valueEnd = queryString.Length;
                }
                else
                {
                    // next pair is right after ampersand
                    pairPos = valueEnd + 1;
                }
                // unescape string
                value = Uri.UnescapeDataString(queryString.Substring(valuePos, valueEnd - valuePos));

                // add unescaped strings to list
                queryList.Add(new KeyValuePair<String, String>(name, value));
                // if this was signaled, at end of querystring
                if (pairPos == -1)
                {
                    break;
                }
            }
        }
    }
}

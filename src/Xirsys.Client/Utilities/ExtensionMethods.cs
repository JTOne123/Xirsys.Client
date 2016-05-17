using System;
using System.Collections.Generic;
using System.Linq;
using Xirsys.Client.Models.REST;

namespace Xirsys.Client.Utilities
{
    public static class ExtensionMethods
    {
        public static bool IsOk<TData>(this XirsysResponseModel<TData> response)
        {
            return String.Equals(response.Status, SystemMessages.OK_STATUS, StringComparison.CurrentCultureIgnoreCase);
        }

        public static String ToHttpString(this List<KeyValuePair<String, String>> collection)
        {
            if (collection == null)
            {
                return String.Empty;
            }

            // simplified version of what ToString in HttpValueCollection does
            var items = new List<String>(collection.Count);
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
    }
}

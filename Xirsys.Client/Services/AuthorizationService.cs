using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xirsys.Client.Models.REST;
using Xirsys.Client.Models.REST.Wire;
using Xirsys.Client.Utilities;

namespace Xirsys.Client
{
    public partial class XirsysApiClient
    {
        protected const String AUTH_SERVICE = "_auth";

        public Task<XirsysResponseModel<List<String>>> ListBlockIpAddressesAsync(String path)
        {
            return InternalGetAsync<List<String>>(GetServiceMethodPath(AUTH_SERVICE, path));
        }

        public Task<XirsysResponseModel<DataVersionResponse<TData>>> GetBlockIpAddressAsync<TData>(String path, String ipAddress)
        {
            Func<String, JObject, XirsysResponseModel<DataVersionResponse<TData>>> okSerializeFunc;
            if (typeof(TData).IsSimpleType())
            {
                okSerializeFunc = SimpleDataParseResponseWithVersion<TData>;
            }
            else
            {
                okSerializeFunc = DataParseResponseWithVersion<TData>;
            }

            return InternalGetAsync(GetServiceMethodPath(AUTH_SERVICE, path),
                new QueryStringList(1)
                    {
                        { "k", ipAddress }
                    },
                okParseResponse: okSerializeFunc);
        }

        public Task<XirsysResponseModel<DataVersionResponse<Object>>> AddBlockIpAddressAsyncAsync(String path, String ipAddress)
        {
            return AddBlockIpAddressAsyncAsync(path, ipAddress, new Object(), null);
        }

        public Task<XirsysResponseModel<DataVersionResponse<Object>>> AddBlockIpAddressAsyncAsync(String path, String ipAddress, String oldVersion)
        {
            return AddBlockIpAddressAsyncAsync(path, ipAddress, new Object(), oldVersion);
        }

        public Task<XirsysResponseModel<DataVersionResponse<TData>>> AddBlockIpAddressAsyncAsync<TData>(String path, String ipAddress, TData note)
        {
            return AddBlockIpAddressAsyncAsync(path, ipAddress, note, null);
        }

        public Task<XirsysResponseModel<DataVersionResponse<TData>>> AddBlockIpAddressAsyncAsync<TData>(String path, String ipAddress, TData note, String oldVersion)
        {
            KeyValueModel<Object> blockIpObj;
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
                            Log.LogWarning("Failed to locate {0} property in Content model. Cannot insert {1} property.", KeyValueModel<Object>.VALUE_PROP, VersionResponse.VERSION_PROP);
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

            if (note != null && note.GetType().IsSimpleType())
            {
                blockIpObj = new KeyValueModel<Object>(ipAddress, new SimpleDataWrapper<TData>(note));
                okParseFunc = SimpleDataParseResponseWithVersion<TData>;
            }
            else
            {
                blockIpObj = new KeyValueModel<Object>(ipAddress, note);
                okParseFunc = DataParseResponseWithVersion<TData>;
            }

            return InternalPutAsync(GetServiceMethodPath(AUTH_SERVICE, path), blockIpObj, serializeContentData: serializeDataFunc, okParseResponse: okParseFunc);
        }

        public Task<XirsysResponseModel<Int32>> RemoveBlockIpAddressAsync(String path, String ipAddress)
        {
            return InternalDeleteAsync<Int32>(GetServiceMethodPath(AUTH_SERVICE, path),
                new QueryStringList(1)
                    {
                        { "k", ipAddress }
                    });
        }

        public Task<XirsysResponseModel<Object>> RemoveAllBlockIpAddressesAsync(String path)
        {
            return InternalDeleteAsync<Object>(GetServiceMethodPath(AUTH_SERVICE, path));
        }
    }
}

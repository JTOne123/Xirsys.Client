using System;
using System.Collections.Generic;
using System.Threading;
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

        public Task<XirsysResponseModel<List<String>>> ListBlockIpAddressesAsync(String path, 
            CancellationToken cancelToken = default(CancellationToken))
        {
            return InternalGetAsync<List<String>>(GetServiceMethodPath(AUTH_SERVICE, path),
                cancelToken: cancelToken);
        }

        public Task<XirsysResponseModel<DataVersionResponse<TData>>> GetBlockIpAddressAsync<TData>(String path, String ipAddress, 
            CancellationToken cancelToken = default(CancellationToken))
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
                okParseResponse: okSerializeFunc,
                cancelToken: cancelToken);
        }

        public Task<XirsysResponseModel<DataVersionResponse<Object>>> AddBlockIpAddressAsyncAsync(String path, String ipAddress, 
            CancellationToken cancelToken = default(CancellationToken))
        {
            return AddBlockIpAddressAsyncAsync(path, ipAddress, new Object(), null,
                cancelToken: cancelToken);
        }

        public Task<XirsysResponseModel<DataVersionResponse<Object>>> AddBlockIpAddressAsyncAsync(String path, String ipAddress, String oldVersion, 
            CancellationToken cancelToken = default(CancellationToken))
        {
            return AddBlockIpAddressAsyncAsync(path, ipAddress, new Object(), oldVersion,
                cancelToken: cancelToken);
        }

        public Task<XirsysResponseModel<DataVersionResponse<TData>>> AddBlockIpAddressAsyncAsync<TData>(String path, String ipAddress, TData note, 
            CancellationToken cancelToken = default(CancellationToken))
        {
            return AddBlockIpAddressAsyncAsync(path, ipAddress, note, null,
                cancelToken: cancelToken);
        }

        public Task<XirsysResponseModel<DataVersionResponse<TData>>> AddBlockIpAddressAsyncAsync<TData>(String path, String ipAddress, TData note, String oldVersion, 
            CancellationToken cancelToken = default(CancellationToken))
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

            return InternalPutAsync(GetServiceMethodPath(AUTH_SERVICE, path), blockIpObj, serializeContentData: serializeDataFunc, okParseResponse: okParseFunc,
                cancelToken: cancelToken);
        }

        public Task<XirsysResponseModel<Int32>> RemoveBlockIpAddressAsync(String path, String ipAddress, 
            CancellationToken cancelToken = default(CancellationToken))
        {
            return InternalDeleteAsync<Int32>(GetServiceMethodPath(AUTH_SERVICE, path),
                new QueryStringList(1)
                    {
                        { "k", ipAddress }
                    },
                cancelToken: cancelToken);
        }

        public Task<XirsysResponseModel<Object>> RemoveAllBlockIpAddressesAsync(String path, 
            CancellationToken cancelToken = default(CancellationToken))
        {
            return InternalDeleteAsync<Object>(GetServiceMethodPath(AUTH_SERVICE, path),
                cancelToken: cancelToken);
        }
    }
}

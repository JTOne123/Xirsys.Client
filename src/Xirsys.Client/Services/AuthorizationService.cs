using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xirsys.Client.Models.REST;
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

        public Task<XirsysResponseModel<TData>> GetBlockIpAddressAsync<TData>(String path, String ipAddress)
        {
            return InternalGetAsync<TData>(GetServiceMethodPath(AUTH_SERVICE, path),
                new QueryStringList(1)
                    {
                        { "k", ipAddress }
                    });
        }

        public Task<XirsysResponseModel<NullData>> AddBlockIpAddressAsyncAsync(String path, String ipAddress)
        {
            return AddBlockIpAddressAsyncAsync(path, ipAddress, String.Empty);
        }

        public Task<XirsysResponseModel<NullData>> AddBlockIpAddressAsyncAsync<TData>(String path, String ipAddress, TData note)
        {
            return InternalPutAsync<Object, NullData>(GetServiceMethodPath(AUTH_SERVICE, path),
                new { k = ipAddress, v = note });
        }

        public Task<XirsysResponseModel<Int32>> RemoveBlockIpAddressAsync(String path, String ipAddress)
        {
            return InternalDeleteAsync<Int32>(GetServiceMethodPath(AUTH_SERVICE, path),
                new QueryStringList(1)
                    {
                        { "k", ipAddress }
                    });
        }

        public Task<XirsysResponseModel<NullData>> RemoveAllBlockIpAddressesAsync(String path)
        {
            return InternalDeleteAsync<NullData>(GetServiceMethodPath(AUTH_SERVICE, path));
        }
    }
}

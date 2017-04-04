using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xirsys.Client.Models.REST;
using Xirsys.Client.Models.REST.Wire;
using Xirsys.Client.Utilities;

namespace Xirsys.Client
{
    public partial class XirsysApiClient
    {
        protected const String NAMESPACE_SERVICE = "_ns";

        public Task<XirsysResponseModel<VersionResponse>> AddPathAsync(String path)
        {
            return InternalPutAsync<Object, VersionResponse>(GetServiceMethodPath(NAMESPACE_SERVICE, path));
        }

        public Task<XirsysResponseModel<Int32>> RemovePathAsync(String path)
        {
            return InternalDeleteAsync<Int32>(GetServiceMethodPath(NAMESPACE_SERVICE, path));
        }

        public Task<XirsysResponseModel<List<String>>> GetPathNodesAsync(String path, Nullable<Int32> depth = null)
        {
            QueryStringList qsParameters = null;
            if (depth.HasValue)
            {
                qsParameters = new QueryStringList(1)
                    {
                        { "depth", depth.Value.ToString() }
                    };
            }
            return InternalGetAsync<List<String>>(GetServiceMethodPath(NAMESPACE_SERVICE, path), qsParameters);
        }
    }
}

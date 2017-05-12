using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xirsys.Client.Models.REST;
using Xirsys.Client.Models.REST.Wire;
using Xirsys.Client.Utilities;

namespace Xirsys.Client
{
    public partial class XirsysApiClient
    {
        protected const String NAMESPACE_SERVICE = "_ns";

        public Task<XirsysResponseModel<VersionResponse>> AddPathAsync(String path, 
            CancellationToken cancelToken = default(CancellationToken))
        {
            return InternalPutAsync<Object, VersionResponse>(GetServiceMethodPath(NAMESPACE_SERVICE, path),
                cancelToken: cancelToken);
        }

        public Task<XirsysResponseModel<Int32>> RemovePathAsync(String path, 
            CancellationToken cancelToken = default(CancellationToken))
        {
            return InternalDeleteAsync<Int32>(GetServiceMethodPath(NAMESPACE_SERVICE, path),
                cancelToken: cancelToken);
        }

        public Task<XirsysResponseModel<List<String>>> GetPathNodesAsync(String path, Nullable<Int32> depth = null, 
            CancellationToken cancelToken = default(CancellationToken))
        {
            QueryStringList qsParameters = null;
            if (depth.HasValue)
            {
                qsParameters = new QueryStringList(1)
                    {
                        { "depth", depth.Value.ToString() }
                    };
            }
            return InternalGetAsync<List<String>>(GetServiceMethodPath(NAMESPACE_SERVICE, path), qsParameters,
                cancelToken: cancelToken);
        }
    }
}

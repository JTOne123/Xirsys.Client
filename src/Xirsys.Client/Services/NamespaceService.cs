using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xirsys.Client.Models.REST;
using Xirsys.Client.Utilities;

namespace Xirsys.Client
{
    public partial class XirsysApiClient
    {
        protected const String NAMESPACE_SERVICE = "_ns";

        public Task<XirsysResponseModel<NullData>> AddPathAsync(String path)
        {
            return InternalPutAsync<Object, NullData>(GetServiceMethodPath(NAMESPACE_SERVICE, path));
        }

        public Task<XirsysResponseModel<NullData>> RemovePathAsync(String path)
        {
            return InternalDeleteAsync<NullData>(GetServiceMethodPath(NAMESPACE_SERVICE, path));
        }

        public Task<XirsysResponseModel<List<String>>> GetPathNodesAsync(String path, Nullable<Int32> depth = null)
        {
            KeyValueList<String, String> qsParameters = null;
            if (depth.HasValue)
            {
                qsParameters = new KeyValueList<String, String>(1)
                    {
                        { "depth", depth.Value.ToString() }
                    };
            }
            return InternalGetAsync<List<String>>(GetServiceMethodPath(NAMESPACE_SERVICE, path), qsParameters);
        }
    }
}

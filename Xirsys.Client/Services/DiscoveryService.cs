using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xirsys.Client.Models.REST;
using Xirsys.Client.Serialization;
using Xirsys.Client.Utilities;

namespace Xirsys.Client
{
    public partial class XirsysApiClient
    {
        protected const String DISCOVERY_SERVICE = "_ds";

        public Task<XirsysResponseModel<List<String>>> ListLayersAsync()
        {
            return InternalGetAsync<List<String>>(GetServiceMethodPath(DISCOVERY_SERVICE), 
                new QueryStringList(1)
                    {
                        { "a", DiscoveryAction.Layers.ToStringValue() }
                    });
        }

        public Task<XirsysResponseModel<List<String>>> ListPathsAsync(String layer)
        {
            return InternalGetAsync<List<String>>(GetServiceMethodPath(DISCOVERY_SERVICE), 
                new QueryStringList(2)
                    {
                        { "a", DiscoveryAction.Paths.ToStringValue() },
                        { "l", layer }
                    });
        }
    }
}

using System;
using System.Threading;
using System.Threading.Tasks;
using Xirsys.Client.Models.REST;
using Xirsys.Client.Utilities;

namespace Xirsys.Client
{
    public partial class XirsysApiClient
    {
        protected const String HOST_SERVICE = "_host";

        public Task<XirsysResponseModel<String>> GetBestSignalServerAsync(String path = null, CancellationToken cancelToken = default(CancellationToken))
        {
            var qs = new QueryStringList(1)
                {
                    { "type", "signal"},
                    // wtf does this do
                    // { "k", name },
                };
            return InternalGetAsync<String>(GetServiceMethodPath(HOST_SERVICE, path ?? String.Empty), qs,
                cancelToken: cancelToken);
        }

        /*
        public Task<XirsysResponseModel<String>> GetBestTurnServerAsync()
        {
            return InternalGetAsync<String>($"/{HOST_SERVICE}/best/turn");
        }
        */
    }
}

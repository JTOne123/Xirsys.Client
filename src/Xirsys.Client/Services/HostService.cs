using System;
using System.Threading.Tasks;
using Xirsys.Client.Models.REST;

namespace Xirsys.Client
{
    public partial class XirsysApiClient
    {
        protected const String HOST_SERVICE = "_host";

        public Task<XirsysResponseModel<String>> GetBestSignalServerAsync()
        {
            return InternalGetAsync<String>($"/{HOST_SERVICE}/best/signal");
        }

        // failing
        public Task<XirsysResponseModel<String>> GetBestTurnServerAsync()
        {
            return InternalGetAsync<String>($"/{HOST_SERVICE}/best/turn");
        }
    }
}

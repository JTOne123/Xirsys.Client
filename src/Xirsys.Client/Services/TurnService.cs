using System;
using System.Threading.Tasks;
using Xirsys.Client.Models.REST;

namespace Xirsys.Client
{
    public partial class XirsysApiClient
    {
        protected const String TURN_SERVICE = "_turn";

        public Task<XirsysResponseModel<TurnServersResponse>> ListTurnServersAsync(String path)
        {
            return InternalPutAsync<NullData, TurnServersResponse>(GetServiceMethodPath(TURN_SERVICE, path));
        }
    }
}

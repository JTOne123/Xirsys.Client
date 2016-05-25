using System;
using System.Threading.Tasks;
using Xirsys.Client.Models.REST;
using Xirsys.Client.Utilities;

namespace Xirsys.Client
{
    public partial class XirsysApiClient
    {
        protected const String TOKEN_SERVICE = "_token";

        public Task<XirsysResponseModel<String>> GetTokenAsync(String path, String clientId, Nullable<Int32> expire = null)
        {
            var qsParameters = new QueryStringList(2);

            qsParameters.Add("k", clientId);
            if (expire.HasValue)
            {
                qsParameters.Add("expire", expire.Value.ToString());
            }
            var methodPath = GetServiceMethodPath(TOKEN_SERVICE, path) + "?" + qsParameters.ToHttpString();
            return InternalPutAsync<NullData, String>(methodPath);
        }
    }
}

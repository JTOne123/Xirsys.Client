using System;
using System.Threading.Tasks;
using Xirsys.Client.Models.REST;
using Xirsys.Client.Utilities;

namespace Xirsys.Client
{
    public partial class XirsysApiClient
    {
        protected const String TOKEN_SERVICE = "_token";

        // the documentation currently shows default expire to be 60 seconds
        public const Int32 DEFAULT_TOKEN_EXPIRE = 60;

        public Task<XirsysResponseModel<String>> CreateTokenAsync(String path, String clientId = null, Nullable<Int32> expire = null)
        {
            var qsParameters = new QueryStringList(2);
            if (!String.IsNullOrEmpty(clientId))
            {
                qsParameters.Add("k", clientId);
            }
            if (expire.HasValue)
            {
                qsParameters.Add("expire", expire.Value.ToString());
            }
            var methodPath = GetServiceMethodPath(TOKEN_SERVICE, path) + "?" + qsParameters.ToHttpString();
            return InternalPutAsync<Object, String>(methodPath);
        }
    }
}

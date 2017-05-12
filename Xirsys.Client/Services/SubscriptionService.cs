using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xirsys.Client.Models.REST;
using Xirsys.Client.Utilities;

namespace Xirsys.Client
{
    public partial class XirsysApiClient
    {
        protected const String SUBSCRIPTION_SERVICE = "_subs";


        // topics are created upon a websocket connection, you do not manually create them
        //public Task<XirsysResponseModel<Object>> AddTopicAsync(String path, String topic, 
        //    CancellationToken cancelToken = default(CancellationToken))
        //{
        //    throw new NotImplementedException();
        //}

        public Task<XirsysResponseModel<Int32>> RemoveTopicAsync(String path, String topic, 
            CancellationToken cancelToken = default(CancellationToken))
        {
            return InternalDeleteAsync<Int32>(GetServiceMethodPath(SUBSCRIPTION_SERVICE, path, topic),
                cancelToken: cancelToken);
        }

        public Task<XirsysResponseModel<List<Object>>> GetTopicAsync(String path, String topic, 
            CancellationToken cancelToken = default(CancellationToken))
        {
            return InternalGetAsync<List<Object>>(GetServiceMethodPath(SUBSCRIPTION_SERVICE, path, topic),
                new QueryStringList(1)
                    {
                        { "as", "values" }
                    },
                cancelToken: cancelToken);
        }

        public Task<XirsysResponseModel<List<String>>> ListTopicKeysAsync(String path, String topic, 
            CancellationToken cancelToken = default(CancellationToken))
        {
            return InternalGetAsync<List<String>>(GetServiceMethodPath(SUBSCRIPTION_SERVICE, path, topic),
                cancelToken: cancelToken);
        }


        public Task<XirsysResponseModel<Object>> GetSubscriptionUserAsync(String path, String topic, String clientId, 
            CancellationToken cancelToken = default(CancellationToken))
        {
            return InternalGetAsync<Object>(GetServiceMethodPath(SUBSCRIPTION_SERVICE, path, topic),
                new QueryStringList(1)
                    {
                        { "k", clientId }
                    },
                cancelToken: cancelToken);
        }

        public Task<XirsysResponseModel<Int32>> KickSubscriptionUserAsync(String path, String topic, String clientId, 
            CancellationToken cancelToken = default(CancellationToken))
        {
            return InternalDeleteAsync<Int32>(GetServiceMethodPath(SUBSCRIPTION_SERVICE, path, topic), 
                new QueryStringList(1)
                    {
                        { "k", clientId }
                    },
                cancelToken: cancelToken);
        }
    }
}

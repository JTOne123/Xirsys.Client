using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xirsys.Client.Models.REST;
using Xirsys.Client.Utilities;

namespace Xirsys.Client
{
    public partial class XirsysApiClient
    {
        protected const String SUBSCRIPTION_SERVICE = "_subs";


        // topics are created upon a websocket connection, you do not manually create them
        //public Task<XirsysResponseModel<Object>> AddTopicAsync(String path, String topic)
        //{
        //    throw new NotImplementedException();
        //}

        public Task<XirsysResponseModel<Int32>> RemoveTopicAsync(String path, String topic)
        {
            return InternalDeleteAsync<Int32>(GetServiceMethodPath(SUBSCRIPTION_SERVICE, path, topic));
        }

        public Task<XirsysResponseModel<List<Object>>> GetTopicAsync(String path, String topic)
        {
            return InternalGetAsync<List<Object>>(GetServiceMethodPath(SUBSCRIPTION_SERVICE, path, topic),
                new QueryStringList(1)
                    {
                        { "as", "values" }
                    });
        }

        public Task<XirsysResponseModel<List<String>>> ListTopicKeysAsync(String path, String topic)
        {
            return InternalGetAsync<List<String>>(GetServiceMethodPath(SUBSCRIPTION_SERVICE, path, topic));
        }


        public Task<XirsysResponseModel<Object>> GetSubscriptionUserAsync(String path, String topic, String clientId)
        {
            return InternalGetAsync<Object>(GetServiceMethodPath(SUBSCRIPTION_SERVICE, path, topic),
                new QueryStringList(1)
                    {
                        { "k", clientId }
                    });
        }

        public Task<XirsysResponseModel<Int32>> KickSubscriptionUserAsync(String path, String topic, String clientId)
        {
            return InternalDeleteAsync<Int32>(GetServiceMethodPath(SUBSCRIPTION_SERVICE, path, topic), 
                new QueryStringList(1)
                    {
                        { "k", clientId }
                    });
        }
    }
}

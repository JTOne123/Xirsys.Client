using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xirsys.Client.Models.REST;
using Xirsys.Client.Models.REST.Wire;
using Xirsys.Client.Utilities;

namespace Xirsys.Client
{
    public partial class XirsysApiClient
    {
        protected const String ACCOUNT_SERVICE = "_acc";
        protected const String ACCOUNT_ADMIN_SERVICE = ACCOUNT_SERVICE + "/users/admins";
        protected const String ACCOUNT_SUBACCOUNTS_SERVICE = ACCOUNT_SERVICE + "/accounts";

        // failing
        public async Task<XirsysResponseModel<DataVersionResponse<AdminModel>>> AddAdminAsync(String adminName, BaseAdminModel adminAccount)
        {
            var apiResponse = await InternalPutAsync(GetServiceMethodPath(ACCOUNT_ADMIN_SERVICE), 
                new KeyValueModel<BaseAdminModel>(adminName, adminAccount), okParseResponse: DataParseResponseWithVersion<AdminModel>);

            if (apiResponse.IsOk())
            {
                // necessary: not returned by webapi
                apiResponse.Data.Data.UserName = adminName;
            }

            return apiResponse;
        }

        public Task<XirsysResponseModel<Int32>> RemoveAdminAsync(String adminName)
        {
            return InternalDeleteAsync<Int32>(GetServiceMethodPath(ACCOUNT_ADMIN_SERVICE), 
                new KeyValueList<String, String>(1)
                    {
                        { "k", adminName }
                    });
        }

        public async Task<XirsysResponseModel<DataVersionResponse<AdminModel>>> GetAdminAsync(String adminName)
        {
            var apiResponse = await InternalGetAsync(GetServiceMethodPath(ACCOUNT_ADMIN_SERVICE),
                new KeyValueList<String, String>(1)
                    {
                        { "k", adminName }
                    }, okParseResponse: DataParseResponseWithVersion<AdminModel>);

            if (apiResponse.IsOk())
            {
                apiResponse.Data.Data.UserName = adminName;
            }

            return apiResponse;
        }

        // unsure of return type
        public Task<XirsysResponseModel<Object>> GetAdminTimeSeriesAsync(String adminName)
        {
            return InternalGetAsync<Object>(GetServiceMethodPath(ACCOUNT_ADMIN_SERVICE),
                new KeyValueList<String, String>(1)
                    {
                        { "k", adminName },
                        { "time_series", "1" },
                    });
        }

        public Task<XirsysResponseModel<List<String>>> ListAdminsAsync()
        {
            return InternalGetAsync<List<String>>(GetServiceMethodPath(ACCOUNT_ADMIN_SERVICE));
        }

        public Task<XirsysResponseModel<Object>> UpdateAdminAsync(String adminName, BaseAdminModel modifyAdminProps)
        {
            throw new NotImplementedException();
        }



        public Task<XirsysResponseModel<DataVersionResponse<SubAccountModel>>> AddSubAccountAsync(String userName, BaseSubAccountModel subAccount)
        {
            return InternalPutAsync<Object, DataVersionResponse<SubAccountModel>>(GetServiceMethodPath(ACCOUNT_SUBACCOUNTS_SERVICE), 
                new KeyValueModel<BaseSubAccountModel>(userName, subAccount),
                okParseResponse: DataParseResponseWithVersion<SubAccountModel>);
        }

        public Task<XirsysResponseModel<Int32>> RemoveSubAccountAsync(String userName)
        {
            return InternalDeleteAsync<Int32>(GetServiceMethodPath(ACCOUNT_SUBACCOUNTS_SERVICE), 
                new QueryStringList(1)
                    {
                        { "k", userName }
                    });
        }

        public Task<XirsysResponseModel<DataVersionResponse<SubAccountModel>>> GetSubAccountByUserNameAsync(String userName)
        {
            return InternalGetAsync(GetServiceMethodPath(ACCOUNT_SUBACCOUNTS_SERVICE),
                new QueryStringList(1)
                    {
                        { "k", userName }
                    },
                okParseResponse: DataParseResponseWithVersion<SubAccountModel>);
        }

        public Task<XirsysResponseModel<DataVersionResponse<SubAccountModel>>> GetSubAccountByEmailAsync(String email)
        {
            return InternalGetAsync(GetServiceMethodPath(ACCOUNT_SUBACCOUNTS_SERVICE),
                new QueryStringList(1)
                    {
                        { "k2", email }
                    },
                okParseResponse: DataParseResponseWithVersion<SubAccountModel>);
        }

        public Task<XirsysResponseModel<List<String>>> ListSubAccountsAsync()
        {
            return InternalGetAsync<List<String>>(GetServiceMethodPath(ACCOUNT_SUBACCOUNTS_SERVICE));
        }

        public Task<XirsysResponseModel<DataVersionResponse<SubAccountModel>>> UpdateSubAccountAsync(String userName, UpdateSubAccountModel modifySubAccountProps)
        {
            // not documented but it was mentioned in passing that if a field is serialized as null on the wire
            // the API would remove the field, otherwise only fields sent are modified
            // right now our XirsysApiClient ignores null fields (does not serialize) so this can't be used yet

            // cannot modify username
            return InternalPostAsync<KeyValueModel<UpdateSubAccountModel>, DataVersionResponse<SubAccountModel>>(GetServiceMethodPath(ACCOUNT_SUBACCOUNTS_SERVICE),
                new KeyValueModel<UpdateSubAccountModel>(userName, modifySubAccountProps), okParseResponse: DataParseResponseWithVersion<SubAccountModel>);
        }
    }
}

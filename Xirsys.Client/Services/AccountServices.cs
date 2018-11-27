using System;
using System.Collections.Generic;
using System.Threading;
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
        public async Task<XirsysResponseModel<DataVersionResponse<AdminModel>>> AddAdminAsync(String adminName, BaseAdminModel adminAccount, 
            CancellationToken cancelToken = default(CancellationToken))
        {
            var apiResponse = await InternalPutAsync(GetServiceMethodPath(ACCOUNT_ADMIN_SERVICE), 
                new KeyValueModel<BaseAdminModel>(adminName, adminAccount), okParseResponse: DataParseResponseWithVersion<AdminModel>,
                cancelToken: cancelToken);

            if (apiResponse.IsOk())
            {
                // necessary: not returned by webapi
                apiResponse.Data.Data.UserName = adminName;
            }

            return apiResponse;
        }

        public Task<XirsysResponseModel<Int32>> RemoveAdminAsync(String adminName, 
            CancellationToken cancelToken = default(CancellationToken))
        {
            return InternalDeleteAsync<Int32>(GetServiceMethodPath(ACCOUNT_ADMIN_SERVICE), 
                new KeyValueList<String, String>(1)
                    {
                        { "k", adminName }
                    },
                cancelToken: cancelToken);
        }

        public async Task<XirsysResponseModel<DataVersionResponse<AdminModel>>> GetAdminAsync(String adminName, 
            CancellationToken cancelToken = default(CancellationToken))
        {
            var apiResponse = await InternalGetAsync(GetServiceMethodPath(ACCOUNT_ADMIN_SERVICE),
                new KeyValueList<String, String>(1)
                    {
                        { "k", adminName }
                    }, 
                okParseResponse: DataParseResponseWithVersion<AdminModel>,
                cancelToken: cancelToken);

            if (apiResponse.IsOk())
            {
                apiResponse.Data.Data.UserName = adminName;
            }

            return apiResponse;
        }

        // unsure of return type
        public Task<XirsysResponseModel<Object>> GetAdminTimeSeriesAsync(String adminName, 
            CancellationToken cancelToken = default(CancellationToken))
        {
            return InternalGetAsync<Object>(GetServiceMethodPath(ACCOUNT_ADMIN_SERVICE),
                new KeyValueList<String, String>(1)
                    {
                        { "k", adminName },
                        { "time_series", "1" },
                    },
                cancelToken: cancelToken);
        }

        public Task<XirsysResponseModel<List<String>>> ListAdminsAsync(CancellationToken cancelToken = default(CancellationToken))
        {
            return InternalGetAsync<List<String>>(GetServiceMethodPath(ACCOUNT_ADMIN_SERVICE),
                cancelToken: cancelToken);
        }

        public Task<XirsysResponseModel<Object>> UpdateAdminAsync(String adminName, BaseAdminModel modifyAdminProps,
            CancellationToken cancelToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }



        public Task<XirsysResponseModel<DataVersionResponse<TAccountModel>>> AddSubAccountAsync<TBaseModel, TAccountModel>(String userName, TBaseModel subAccount, 
            CancellationToken cancelToken = default(CancellationToken))
            where TBaseModel : BaseSubAccountModel
            where TAccountModel : SubAccountModel
        {
            return InternalPutAsync<Object, DataVersionResponse<TAccountModel>>(GetServiceMethodPath(ACCOUNT_SUBACCOUNTS_SERVICE), 
                new KeyValueModel<BaseSubAccountModel>(userName, subAccount),
                okParseResponse: DataParseResponseWithVersion<TAccountModel>,
                cancelToken: cancelToken);
        }

        public Task<XirsysResponseModel<Int32>> RemoveSubAccountAsync(String userName, 
            CancellationToken cancelToken = default(CancellationToken))
        {
            return InternalDeleteAsync<Int32>(GetServiceMethodPath(ACCOUNT_SUBACCOUNTS_SERVICE), 
                new QueryStringList(1)
                    {
                        { "k", userName }
                    },
                cancelToken: cancelToken);
        }

        public Task<XirsysResponseModel<DataVersionResponse<TAccountModel>>> GetSubAccountByUserNameAsync<TAccountModel>(String userName, 
            CancellationToken cancelToken = default(CancellationToken))
            where TAccountModel : SubAccountModel
        {
            return InternalGetAsync(GetServiceMethodPath(ACCOUNT_SUBACCOUNTS_SERVICE),
                new QueryStringList(1)
                    {
                        { "k", userName }
                    },
                okParseResponse: DataParseResponseWithVersion<TAccountModel>,
                cancelToken: cancelToken);
        }

        public Task<XirsysResponseModel<DataVersionResponse<TAccountModel>>> GetSubAccountByEmailAsync<TAccountModel>(String email, 
            CancellationToken cancelToken = default(CancellationToken))
            where TAccountModel : SubAccountModel
        {
            return InternalGetAsync(GetServiceMethodPath(ACCOUNT_SUBACCOUNTS_SERVICE),
                new QueryStringList(1)
                    {
                        { "k2", email }
                    },
                okParseResponse: DataParseResponseWithVersion<TAccountModel>,
                cancelToken: cancelToken);
        }

        public Task<XirsysResponseModel<List<String>>> ListSubAccountsAsync(CancellationToken cancelToken = default(CancellationToken))
        {
            return InternalGetAsync<List<String>>(GetServiceMethodPath(ACCOUNT_SUBACCOUNTS_SERVICE),
                cancelToken: cancelToken);
        }

        public Task<XirsysResponseModel<List<DatumResponse<TAccountModel>>>> ListSubAccountValuesAsync<TAccountModel>(CancellationToken cancelToken = default(CancellationToken))
            where TAccountModel : SubAccountModel
        {
            return InternalGetAsync<List<DatumResponse<TAccountModel>>>(GetServiceMethodPath(ACCOUNT_SUBACCOUNTS_SERVICE),
                new QueryStringList(1)
                    {
                        {"as", "datum"}
                    },
                okParseResponse: ListDatumParseResponse<TAccountModel>,
                cancelToken: cancelToken);
        }

        public Task<XirsysResponseModel<DataVersionResponse<TAccountModel>>> UpdateSubAccountAsync<TAccountModel, TUpdateAccountModel>(String userName, TUpdateAccountModel modifySubAccountProps,
            Boolean serializeNull = false,
            CancellationToken cancelToken = default(CancellationToken))
            where TAccountModel : SubAccountModel
            where TUpdateAccountModel : class
        {
            // not documented but it was mentioned in passing that if a field is serialized as null on the wire
            // the API would remove the field, otherwise only fields sent are modified
            // right now our XirsysApiClient ignores null fields (does not serialize) so this can't be used yet
            // RE-NOTE: currently sending null will set the field as null, it DOES NOT __remove__ the field from the object (which was the real question above)
            // using the serializeNull parameter we will control whether to serialize nulls or not

            // cannot modify username
            return InternalPostAsync<KeyValueModel<TUpdateAccountModel>, DataVersionResponse<TAccountModel>>(GetServiceMethodPath(ACCOUNT_SUBACCOUNTS_SERVICE),
                new KeyValueModel<TUpdateAccountModel>(userName, modifySubAccountProps), okParseResponse: DataParseResponseWithVersion<TAccountModel>,
                serializeNull: serializeNull,
                cancelToken: cancelToken);
        }
    }
}

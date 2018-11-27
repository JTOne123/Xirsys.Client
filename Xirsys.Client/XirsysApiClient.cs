using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xirsys.Client.Extensions;
using Xirsys.Client.Models.REST;
using Xirsys.Client.Models.REST.Wire;
using Xirsys.Client.Serialization;
using Xirsys.Client.Utilities;

namespace Xirsys.Client
{
    public partial class XirsysApiClient : IDisposable
    {
        static XirsysApiClient()
        {
            RegionGateways = new Dictionary<XirsysRegion, String>()
                {
                    { XirsysRegion.UsaWest,   "https://ws.xirsys.com" },
                    { XirsysRegion.UsaEast,   "https://us.xirsys.com" },
                    { XirsysRegion.Europe,    "https://es.xirsys.com" },
                    { XirsysRegion.Asia,      "https://ss.xirsys.com" },
                    { XirsysRegion.Singapore, "https://ss.xirsys.com" },
                    { XirsysRegion.Australia, "https://ms.xirsys.com" },

                    { XirsysRegion.Shanghai,  "https://sh.xirsys.com" },
                    { XirsysRegion.Bangalore, "https://bs.xirsys.com" },
                    { XirsysRegion.Tokyo,     "https://tk.xirsys.com" },
                };
        }

        public static readonly IDictionary<XirsysRegion, String> RegionGateways;

        public const String STATUS_PROP = "s";
        public const String VALUE_PROP = "v";

        private readonly ILogger Logger;

        protected HttpClient m_HttpClient;

        private bool m_IsDisposed = false;

        public XirsysApiClient(String apiIdent, String apiSecret, ILogger logger)
            : this(RegionGateways.First().Value, apiIdent, apiSecret, logger)
        {
        }

        public XirsysApiClient(XirsysRegion gatewayRegion, String apiIdent, String apiSecret, ILogger logger)
            : this(RegionGateways[gatewayRegion], apiIdent, apiSecret, logger)
        {
        }

        public XirsysApiClient(String apiIdent, String apiSecret, ILogger<XirsysApiClient> logger)
            : this(RegionGateways.First().Value, apiIdent, apiSecret, logger)
        {
        }

        public XirsysApiClient(XirsysRegion gatewayRegion, String apiIdent, String apiSecret, ILogger<XirsysApiClient> logger)
            : this(RegionGateways[gatewayRegion], apiIdent, apiSecret, logger)
        {
        }

        public XirsysApiClient(String apiBaseUrl, String apiIdent, String apiSecret, ILogger logger)
        {
            this.Logger = logger;

            if (apiBaseUrl.EndsWith("/"))
            {
                apiBaseUrl = apiBaseUrl.TrimEnd('/');
            }

            this.BaseApiUrl = apiBaseUrl;
            this.Ident = apiIdent;
            this.Secret = apiSecret;
        }

        public XirsysApiClient(XirsysApiClient otherClient, String apiBaseUrl = null, String apiIdent = null, String apiSecret = null)
        {
            this.Logger = otherClient.Logger;

            this.BaseApiUrl = apiBaseUrl ?? otherClient.BaseApiUrl;
            this.Ident = apiIdent ?? otherClient.Ident;
            this.Secret = apiSecret ?? otherClient.Secret;

            this.HttpClient = otherClient.HttpClient;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!m_IsDisposed)
            {
                if (disposing)
                {
                    if (m_HttpClient != null)
                    {
                        m_HttpClient.Dispose();
                        m_HttpClient = null;
                    }

                    m_IsDisposed = true;
                }
            }
        }

        public HttpClient HttpClient
        {
            get
            {
                if (m_HttpClient == null)
                {
                    m_HttpClient = new HttpClient();
                }
                return m_HttpClient;
            }
            set { m_HttpClient = value; }
        }

        public String BaseApiUrl { get; private set; }
        public String Ident { get; private set; }
        public String Secret { get; private set; }

        protected static String GetServiceMethodPath(String service, params String[] paths)
        {
            service = service.TrimEnd('/');
            if (!service.StartsWith("/"))
            {
                service = "/" + service;
            }

            if (paths == null)
            {
                return service;
            }

            var sb = new StringBuilder(service);
            foreach (String path in paths)
            {
                var tempPath = path.TrimEnd('/');
                if (String.IsNullOrEmpty(tempPath))
                {
                    continue;
                }

                if (!tempPath.StartsWith("/"))
                {
                    sb.Append("/");
                }
                sb.Append(tempPath);
            }

            return sb.ToString();
        }

        protected XirsysResponseModel<TResponseData> DefaultParseResponse<TResponseData>(String responseStr)
        {
            return ParseResponse<TResponseData>(responseStr, null, null, null);
        }

        // because xirsys has unreliable response messaging we manually walk some of the json
        // and also allow custom deserialization delegate functions to handle odd differences
        protected XirsysResponseModel<TResponseData> ParseResponse<TResponseData>(String responseStr, 
            Func<String, JObject, XirsysResponseModel<TResponseData>> okParseResponse = null,
            Func<String, JObject, XirsysResponseModel<TResponseData>> errorParseResponse = null,
            Func<String, JObject, XirsysResponseModel<TResponseData>> unknownParseResponse = null)
        {
            var responseJObject = JObject.Parse(responseStr);

            var statusToken = responseJObject[STATUS_PROP];
            if (statusToken == null ||
                statusToken.Type != JTokenType.String)
            {
                // no status prop, should mean this is an error
                // this is a problem on xirsys side that should be reported, it is returning invalid json
                Logger.LogWarning("Invalid Xirsys Api Response. Status property is null or not a string. Response: {0}", responseStr);

                // check if value prop is there for error message, pretty much we have an error response though
                return new XirsysResponseModel<TResponseData>(SystemMessages.ERROR_STATUS,
                    GetErrorValue(responseStr, responseJObject), default(TResponseData), responseStr);
            }

            String statusStr = statusToken.ToObject<String>();
            if (String.Equals(statusStr, SystemMessages.OK_STATUS, StringComparison.CurrentCultureIgnoreCase))
            {
                // we should be fine at this point, we have a status that definitely says OK, attempt deserialization
                if (okParseResponse != null)
                {
                    return okParseResponse(responseStr, responseJObject);
                }
                else
                {
                    // default deserialization
                    return JsonNetExtensions.DeserializeObject<XirsysResponseModel<TResponseData>>(responseStr);
                }
            }
            else if (String.Equals(statusStr, SystemMessages.ERROR_STATUS, StringComparison.CurrentCultureIgnoreCase))
            {
                // this is a definite error response status
                if (errorParseResponse != null)
                {
                    return errorParseResponse(responseStr, responseJObject);
                }
                else
                {
                    // default create response object as error and try to get error value
                    return new XirsysResponseModel<TResponseData>(statusStr,
                        GetErrorValue(responseStr, responseJObject), default(TResponseData), responseStr);
                }
            }
            else
            {
                // in case we need to handle other types of status responses(?) individually
                Logger.LogWarning("Unknown Xirsys Api Status. Response: {0}", responseStr);
                if (unknownParseResponse != null)
                {
                    return unknownParseResponse(responseStr, responseJObject);
                }
                else
                {
                    // default create response object as whatever status is and try to get error value
                    return new XirsysResponseModel<TResponseData>(statusStr,
                        GetErrorValue(responseStr, responseJObject), default(TResponseData), responseStr);
                }
            }
        }

        protected String GetErrorValue(String originalResponseStr, JToken jToken, String defaultValueStr = ErrorMessages.Parsing)
        {
            String valueStr;

            var valueToken = jToken[VALUE_PROP];
            if (valueToken != null &&
                valueToken.Type == JTokenType.String)
            {
                valueStr = valueToken.ToObject<String>();
            }
            else
            {
                // to my knowledge should never be null or NOT a string when there is an error
                Logger.LogWarning("Invalid Xirsys Api Error Response. Value field is null or not a string. Response: {0}", originalResponseStr);
                valueStr = defaultValueStr;
            }

            return valueStr;
        }

        protected async Task<XirsysResponseModel<TResponseData>> InternalSendAsync<TResponseData>(HttpRequestMessage httpRequest, String requestUserName, String requestPassword, 
            Func<String, XirsysResponseModel<TResponseData>> deserializeResponse = null, 
            CancellationToken cancelToken = default(CancellationToken))
        {
            if (deserializeResponse == null)
            {
                deserializeResponse = DefaultParseResponse<TResponseData>;
            }

            var requestUri = httpRequest.RequestUri;
            var requestVerb = httpRequest.Method;
            // backed up for printing purposes
            String httpContentStr = null;
            if (httpRequest.Content != null)
            {
                httpContentStr = await httpRequest.Content.ReadAsStringAsync();
            }

            if (requestUserName != null)
            {
                // if requestPassword is null String.Format will handle that
                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(String.Format("{0}:{1}", requestUserName, requestPassword))));
            }

            String responseStr = null;
            using (var response = await this.HttpClient.SendAsync(httpRequest, cancelToken))
            {
                try
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        responseStr = await response.Content.ReadAsStringAsync();
                        Logger.LogError("RequestUri: {0} HttpVerb: {1} UserName: {2} StatusCode: {3} ReasonPhrase: {4} HttpContent: {5} HttpResponse: {6}",
                            requestUri, requestVerb, requestUserName, response.StatusCode, response.ReasonPhrase, httpContentStr, responseStr);
                        return deserializeResponse(responseStr);
                    }

                    responseStr = await response.Content.ReadAsStringAsync();
                    if (Logger.IsEnabled(LogLevel.Trace))
                    {
                        // can be problematic if there is TOO much to print in httpContentStr
                        Logger.LogTrace("RequestUri: {0} HttpVerb: {1} UserName: {2} HttpContent: {3} HttpResponse: {4}",
                            requestUri, requestVerb, requestUserName, httpContentStr, responseStr);
                    }
                    else if (Logger.IsEnabled(LogLevel.Debug))
                    {
                        Logger.LogDebug("RequestUri: {0} HttpVerb: {1} HttpContent: {2} HttpResponse: {3}",
                            requestUri, requestVerb, httpContentStr.DebugLengthCheck(), responseStr.DebugLengthCheck());
                    }

                    return deserializeResponse(responseStr);
                }
                catch (Exception ex)
                {
                    Logger.LogError(0, ex, "Error parsing response");
                    return new XirsysResponseModel<TResponseData>(SystemMessages.ERROR_STATUS, ErrorMessages.Parsing, default(TResponseData), responseStr);
                }
            }
        }

        protected async Task<XirsysResponseModel<TResponseData>> InternalGetAsync<TResponseData>(String servicePath,
            IEnumerable<KeyValuePair<String, String>> parameters = null, Func<String, JObject, XirsysResponseModel<TResponseData>> okParseResponse = null,
            CancellationToken cancelToken = default(CancellationToken))
        {
            if (!servicePath.StartsWith("/"))
            {
                servicePath = "/" + servicePath;
            }

            var uriBuilder = new UriBuilder(BaseApiUrl + servicePath)
                {
                    Query = parameters.ToHttpString()
                };

            using (var httpReq = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri))
            {
                return await InternalSendAsync(httpReq, this.Ident, this.Secret,
                    (responseStr) => ParseResponse(responseStr, okParseResponse, null, null), cancelToken);
            }
        }

        protected async Task<XirsysResponseModel<TResponseData>> InternalPostAsync<TContentData, TResponseData>(String servicePath,
            TContentData data = default(TContentData), 
            Func<TContentData, String> serializeContentData = null,
            Boolean serializeNull = false,
            Func<String, JObject, XirsysResponseModel<TResponseData>> okParseResponse = null,
            CancellationToken cancelToken = default(CancellationToken))
        {
            if (!servicePath.StartsWith("/"))
            {
                servicePath = "/" + servicePath;
            }
            if (serializeContentData == null)
            {
                serializeContentData = (contentData) => JsonNetExtensions.SerializeObject(contentData, serializeNull);
            }

            using (var httpReq = new HttpRequestMessage(HttpMethod.Post, BaseApiUrl + servicePath))
            {
                httpReq.Content = new StringContent(serializeContentData(data), Encoding.UTF8, "application/json");
                return await InternalSendAsync(httpReq, this.Ident, this.Secret, 
                    (responseStr) => ParseResponse(responseStr, okParseResponse, null, null), cancelToken);
            }
        }

        protected async Task<XirsysResponseModel<TResponseData>> InternalPutAsync<TContentData, TResponseData>(String servicePath,
            TContentData data = default(TContentData),
            Func<TContentData, String> serializeContentData = null,
            Boolean serializeNull = false,
            Func<String, JObject, XirsysResponseModel<TResponseData>> okParseResponse = null,
            CancellationToken cancelToken = default(CancellationToken))
        {
            if (!servicePath.StartsWith("/"))
            {
                servicePath = "/" + servicePath;
            }
            if (serializeContentData == null)
            {
                serializeContentData = (contentData) => JsonNetExtensions.SerializeObject(contentData, serializeNull);
            }

            using (var httpReq = new HttpRequestMessage(HttpMethod.Put, BaseApiUrl + servicePath))
            {
                httpReq.Content = new StringContent(serializeContentData(data), Encoding.UTF8, "application/json");
                return await InternalSendAsync(httpReq, this.Ident, this.Secret, 
                    (responseStr) => ParseResponse(responseStr, okParseResponse, null, null), cancelToken);
            }
        }

        protected async Task<XirsysResponseModel<TResponseData>> InternalDeleteAsync<TResponseData>(String servicePath,
            IEnumerable<KeyValuePair<String, String>> parameters = null, Func<String, JObject, XirsysResponseModel<TResponseData>> okParseResponse = null,
            CancellationToken cancelToken = default(CancellationToken))
        {
            if (!servicePath.StartsWith("/"))
            {
                servicePath = "/" + servicePath;
            }

            var uriBuilder = new UriBuilder(BaseApiUrl + servicePath)
                {
                    Query = parameters.ToHttpString()
                };

            using (var httpReq = new HttpRequestMessage(HttpMethod.Delete, uriBuilder.Uri))
            {
                return await InternalSendAsync(httpReq, this.Ident, this.Secret, 
                    (responseStr) => ParseResponse(responseStr, okParseResponse, null, null), cancelToken);
            }
        }



        protected XirsysResponseModel<List<DatumResponse<TResponseData>>> ListDatumParseResponse<TResponseData>(String responseStr, JObject parsedJObject)
        {
            return ListDatumParseResponse<TResponseData, TResponseData>(responseStr, parsedJObject, (deserialized) => deserialized);
        }

        protected XirsysResponseModel<List<DatumResponse<TResponseData>>> ListDatumParseResponse<TResponseData, TSerializedData>(String responseStr, JObject parsedJObject,
            Func<TSerializedData, TResponseData> serializedToResponseFunc = null)
        {
            var valueToken = parsedJObject[VALUE_PROP];
            if (valueToken == null ||
                valueToken.Type != JTokenType.Array)
            {
                // value should never be null or NOT an array, if it is the service layer has some bugs
                Logger.LogWarning("Invalid Xirsys Api Response. Value property is null or not an array. Response: {0}", responseStr);
                return new XirsysResponseModel<List<DatumResponse<TResponseData>>>(SystemMessages.ERROR_STATUS, ErrorMessages.Parsing, null, responseStr);
            }

            var listOfDatums = new List<DatumResponse<TResponseData>>(valueToken.Count());
            foreach (var valueItem in valueToken)
            {
                var rawDatum = valueItem.ToObject<DatumResponse<TSerializedData>>();
                if (rawDatum == null)
                {
                    // likewise if we can't serialize back to data type, there is a problem
                    Logger.LogWarning("Invalid Xirsys Api Response. Value property did not deserialize to {0}. Response: {1}", typeof(TSerializedData).Name, responseStr);
                    return new XirsysResponseModel<List<DatumResponse<TResponseData>>>(SystemMessages.ERROR_STATUS, ErrorMessages.Parsing, null, responseStr);
                }

                // technically TSerializedData will also contain a _ver_ field, however because this should be identical to the _id we don't really need it
                // at this level and can ignore it
                listOfDatums.Add(DatumResponse<TResponseData>.CopyAndFormatData<TSerializedData>(rawDatum, serializedToResponseFunc));
            }

            return new XirsysResponseModel<List<DatumResponse<TResponseData>>>(SystemMessages.OK_STATUS, listOfDatums, responseStr);
        }



        [Obsolete("Not used anymore, should be parsing Datum models")]
        protected XirsysResponseModel<List<DataVersionResponse<TResponseData>>> ListParseResponseWithVersion<TResponseData>(String responseStr, JObject parsedJObject)
        {
            return ListParseResponseWithVersion<TResponseData, TResponseData>(responseStr, parsedJObject, (deserialized) => deserialized);
        }

        [Obsolete("Not used anymore, should be parsing Datum models")]
        protected XirsysResponseModel<List<DataVersionResponse<TResponseData>>> ListParseResponseWithVersion<TResponseData, TSerializedData>(String responseStr, JObject parsedJObject,
            Func<TSerializedData, TResponseData> serializedToResponseFunc = null)
        {
            var valueToken = parsedJObject[VALUE_PROP];
            if (valueToken == null ||
                valueToken.Type != JTokenType.Array)
            {
                // value should never be null or NOT an array, if it is the service layer has some bugs
                Logger.LogWarning("Invalid Xirsys Api Response. Value property is null or not an array. Response: {0}", responseStr);
                return new XirsysResponseModel<List<DataVersionResponse<TResponseData>>>(SystemMessages.ERROR_STATUS, ErrorMessages.Parsing, null, responseStr);
            }

            var listWithVersions = new List<DataVersionResponse<TResponseData>>(valueToken.Count());
            foreach (var valueItem in valueToken)
            {
                var valueData = valueItem.ToObject<TSerializedData>();
                if (valueData == null)
                {
                    // likewise if we can't serialize back to data type, there is a problem
                    Logger.LogWarning("Invalid Xirsys Api Response. Value property did not deserialize to {0}. Response: {1}", typeof(TSerializedData).Name, responseStr);
                    return new XirsysResponseModel<List<DataVersionResponse<TResponseData>>>(SystemMessages.ERROR_STATUS, ErrorMessages.Parsing, null, responseStr);
                }

                var versionToken = valueItem[VersionResponse.VERSION_PROP];
                String versionValue;
                if (versionToken != null &&
                    versionToken.Type == JTokenType.String)
                {
                    versionValue = versionToken.ToObject<String>();
                    if (String.IsNullOrEmpty(versionValue))
                    {
                        // sort of a problem, but we can continue
                        Logger.LogWarning($"Invalid Xirsys Api Response. {VersionResponse.VERSION_PROP} property was empty. Response: {{1}}", responseStr);
                        versionValue = String.Empty;
                    }
                }
                else
                {
                    // sort of a problem, but we can continue
                    Logger.LogWarning($"Invalid Xirsys Api Response. {VersionResponse.VERSION_PROP} property was not present or invalid type. Response: {{1}}", responseStr);
                    versionValue = String.Empty;
                }

                listWithVersions.Add(
                    new DataVersionResponse<TResponseData>(serializedToResponseFunc(valueData), versionValue)
                );
            }

            return new XirsysResponseModel<List<DataVersionResponse<TResponseData>>>(SystemMessages.OK_STATUS, listWithVersions, responseStr);
        }
    }
}

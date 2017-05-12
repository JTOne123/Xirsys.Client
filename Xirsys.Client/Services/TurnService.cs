using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Xirsys.Client.Models.REST;
using Xirsys.Client.Models.REST.Wire;
using Xirsys.Client.Utilities;

namespace Xirsys.Client
{
    public partial class XirsysApiClient
    {
        protected const String TURN_SERVICE = "_turn";

        public async Task<XirsysResponseModel<List<IceServerModel>>> ListTurnServersAsync(String path, 
            CancellationToken cancelToken = default(CancellationToken))
        {
            var iceServerResponse = await InternalPutAsync<Object, TurnServersResponse>(GetServiceMethodPath(TURN_SERVICE, path),
                cancelToken: cancelToken);

            return new XirsysResponseModel<List<IceServerModel>>(
                iceServerResponse.Status, 
                iceServerResponse.ErrorResponse, 
                ConvertModel(iceServerResponse.Data),
                iceServerResponse.RawHttpResponse);
        }

        public List<IceServerModel> ConvertModel(TurnServersResponse turnServersResponse)
        {
            if (turnServersResponse == null)
            {
                return new List<IceServerModel>();
            }

            return ConvertModel(turnServersResponse.IceServers);
        }

        public List<IceServerModel> ConvertModel(List<ServerModel> wireServerModels)
        {
            if (wireServerModels == null)
            {
                return new List<IceServerModel>();
            }

            var iceServers = new List<IceServerModel>(wireServerModels.Count);
            foreach (var wireServerModel in wireServerModels)
            {
                var serverUrl = wireServerModel.Url;

                var protocolIndex = serverUrl.IndexOf(':');
                if (protocolIndex == -1)
                {
                    Logger.LogWarning("Failed to identity protocol in IceServer Url: {0}", serverUrl);
                    continue;
                }

                var protocolString = serverUrl.Substring(0, protocolIndex);
                NatTraversalProtocol protocolEnum;
                ServerTransportLayer transport;
                Int32 defaultPort = -1;
                bool isSecure = false;
                switch (protocolString)
                {
                    case "stun":
                        protocolEnum = NatTraversalProtocol.Stun;
                        transport = ServerTransportLayer.Udp;
                        defaultPort = 3478;
                        break;
                    case "stuns":
                        protocolEnum = NatTraversalProtocol.Stun;
                        transport = ServerTransportLayer.Tcp;
                        defaultPort = 5349;
                        isSecure = true;
                        break;
                    case "turn":
                        protocolEnum = NatTraversalProtocol.Turn;
                        // doesn't necessarily mean udp, but this is the expected default
                        // we'll still parse transport=
                        transport = ServerTransportLayer.Udp;
                        defaultPort = 3478;
                        break;
                    case "turns":
                        protocolEnum = NatTraversalProtocol.Turn;
                        // same as above, turns: currently represents TLS-over-TCP but is not defined
                        transport = ServerTransportLayer.Tcp;
                        defaultPort = 5349;
                        isSecure = true;
                        break;
                    default:
                        protocolEnum = NatTraversalProtocol.Unknown;
                        transport = ServerTransportLayer.Unknown;
                        defaultPort = 3478; // might as well default to something that seems common'ish
                        Logger.LogWarning("Unknown protocol found in Url: {0}", serverUrl);
                        break;
                }

                Int32 hostEndIndex;
                Int32 portIndex = serverUrl.IndexOf(':', protocolIndex + 1);
                Int32 queryStringIndex = serverUrl.IndexOf('?');

                Int32 port = -1;
                if (portIndex != -1)
                {
                    hostEndIndex = portIndex;
                    Int32 portEndIndex;
                    if (queryStringIndex != -1)
                    {
                        portEndIndex = queryStringIndex - 1;
                        // does queryString come before portIndex (this would be from someone not correctly escaping something)
                        if (queryStringIndex < portIndex)
                        {
                            Logger.LogWarning("Ignoring port. Malformed Url: {0}", serverUrl);
                            // we will give precedence to querystring
                            // and ignore portIndex
                            hostEndIndex = queryStringIndex;

                            // start/end is same which results in empty string
                            portIndex = 0;
                            portEndIndex = 1;
                        }
                    }
                    else
                    {
                        portEndIndex = serverUrl.Length;
                    }

                    var portStr = serverUrl.Substring(portIndex + 1, portEndIndex - portIndex - 1);
                    if (!Int32.TryParse(portStr, out port))
                    {
                        Logger.LogWarning("Malformed Port: {0}", portStr);
                        port = -1;
                    }
                    else if (port < 1 || port > 65535)
                    {
                        // some invalid value
                        Logger.LogWarning("Malformed Port: {0}", portStr);
                        port = -1;
                    }
                }
                else
                {
                    if (queryStringIndex != -1)
                    {
                        // up to querystring
                        hostEndIndex = queryStringIndex;
                    }
                    else
                    {
                        // rest of string is host
                        hostEndIndex = serverUrl.Length;
                    }
                }

                String hostStr = serverUrl.Substring(protocolIndex + 1, hostEndIndex - protocolIndex - 1);

                // stun has no querystring in its uri spec
                if (protocolEnum == NatTraversalProtocol.Turn)
                {
                    if (ParseQueryStringForTransport(serverUrl, isSecure, out ServerTransportLayer queryTransport))
                    {
                        transport = queryTransport;
                    }
                }

                if (port == -1)
                {
                    port = defaultPort;
                }

                var iceServer = new IceServerModel(protocolEnum, transport, isSecure, hostStr, port, 
                    wireServerModel.UserName,
                    wireServerModel.Credential);

                iceServers.Add(iceServer);
            }

            return iceServers;
        }

        protected Boolean ParseQueryStringForTransport(String serverUrl, bool isSecure, out ServerTransportLayer transportLayer)
        {
            transportLayer = ServerTransportLayer.Unknown;

            var queryStringIndex = serverUrl.IndexOf('?');
            if (queryStringIndex == -1)
            {
                // else no queryString present, nothing left to do
                return false;
            }

            var qs = serverUrl.Substring(queryStringIndex).ParseQueryString();
            foreach (var transportKvp in qs.Where(x => String.Equals(x.Key, "transport", StringComparison.CurrentCultureIgnoreCase)))
            {
                switch (transportKvp.Value)
                {
                    case "udp":
                        transportLayer = ServerTransportLayer.Udp;
                        // since we are only parsing for transports and we will give precedence to the first
                        // can return now
                        return true;
                    case "tcp":
                        transportLayer = ServerTransportLayer.Tcp;
                        return true;
                    default:
                        // do nothing
                        Logger.LogWarning("Unknown Transport specified in queryString: {0}", transportKvp.Value);
                        break;
                }
            }

            return false;
        }
    }
}

namespace Xirsys.Client.Models.WebSocket
{
    public enum OperationType
    {
        Unknown = 0,

        OnPeers,

        PeerConnected,
        PeerRemoved,


        Message,

        Session,
    }
}

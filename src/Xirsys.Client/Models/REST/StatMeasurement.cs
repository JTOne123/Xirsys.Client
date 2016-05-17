namespace Xirsys.Client.Models.REST
{
    public enum StatMeasurement
    {
        Unknown = 0,

        RouterSubscriptions,
        RouterPackets,

        TurnSessions,
        TurnData,

        StunSessions,
        StunData,

        ApiCalls
    }
}

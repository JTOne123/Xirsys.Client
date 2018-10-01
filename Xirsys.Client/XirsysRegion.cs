using System;

namespace Xirsys.Client
{
    public enum XirsysRegion
    {
        Unknown = 0,

        UsaWest,
        UsaEast,
        Europe,
        [Obsolete("Switch to Singapore")]
        Asia,
        Australia,

        // previously Asia
        Singapore,

        Shanghai,
        Bangalore,
        Tokyo
    }
}

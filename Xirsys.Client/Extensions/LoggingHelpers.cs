using System;
using System.Collections.Generic;
using System.Text;

namespace Xirsys.Client.Extensions
{
    public static class LoggingHelpers
    {
        public const int MAX_LOG_LENGTH = 1024;
        public static readonly String LENGTH_SUFFIX = "LONG_STR_DETECTED";

        public static String DebugLengthCheck(this String str)
        {
            if (str == null)
            {
                return null;
            }

            if (str.Length <= MAX_LOG_LENGTH)
            {
                return str;
            }

            return str.Substring(0, MAX_LOG_LENGTH) + $"...{LENGTH_SUFFIX}";
        }
    }
}

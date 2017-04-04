using System;
using Newtonsoft.Json;
using Xirsys.Client.Serialization;

namespace Xirsys.Demo.NetFramework
{
    public class JsonFormatProvider : IFormatProvider, ICustomFormatter
    {
        public static readonly JsonFormatProvider Current = new JsonFormatProvider();

        public Object GetFormat(Type formatType)
        {
            return (formatType == typeof(ICustomFormatter)) ? this : null;
        }

        public String Format(String format, Object arg, IFormatProvider formatProvider)
        {
            return String.Format("{0}", JsonNetExtensions.SerializeObject(arg));
        }
    }
}

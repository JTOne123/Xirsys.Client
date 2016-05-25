using System;
using System.Collections.Generic;

namespace Xirsys.Client.Utilities
{
    // slightly less typing to specify the generic types <String, String>
    public class QueryStringList : KeyValueList<String, String>
    {
        public QueryStringList()
        {
        }

        public QueryStringList(IEnumerable<KeyValuePair<String, String>> collection) 
            : base(collection)
        {
        }

        public QueryStringList(Int32 capacity) 
            : base(capacity)
        {
        }
    }
}

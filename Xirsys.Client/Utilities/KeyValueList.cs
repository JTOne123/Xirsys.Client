using System;
using System.Collections.Generic;

namespace Xirsys.Client.Utilities
{
    public class KeyValueList<TKey, TValue> : List<KeyValuePair<TKey, TValue>>
    {
        public KeyValueList()
        {
        }

        public KeyValueList(IEnumerable<KeyValuePair<TKey, TValue>> collection) 
            : base(collection)
        {
        }

        public KeyValueList(Int32 capacity) 
            : base(capacity)
        {
        }

        // syntactic sugar for initializing our lists
        public void Add(TKey key, TValue value)
        {
            this.Add(new KeyValuePair<TKey, TValue>(key, value));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Xirsys.Client.Models.REST.Wire
{
    [DataContract]
    public class DatumResponse<TData>
    {
        // account bucket which owns this data
        [DataMember(Name = "w")]
        public String Owner { get; set; }

        [DataMember(Name = "k")]
        public String Key { get; set; }

        [DataMember(Name = "l")]
        public String Layer { get; set; }

        [DataMember(Name = "p")]
        public String Path { get; set; }

        // timestamp of last revision (?) or first (?)
        [DataMember(Name = "t")]
        public long Timestamp { get; set; }

        // additional metadata indexes for this object
        [DataMember(Name = "m")]
        public Dictionary<String, String> Meta { get; set; }

        [DataMember(Name = "v")]
        public TData Data { get; set; }

        // no idea wtf these are
        [DataMember(Name = "s")]
        public String S { get; set; } // always seems to have the value "c"

        [DataMember(Name = "a")]
        public Object A { get; set; }

        // database internal id (actually the _ver_)
        [DataMember(Name = "_id")]
        public String Id { get; set; }

        // revision id in db (?)
        [DataMember(Name = "_rev")]
        public String Rev { get; set; }

        public DatumResponse()
        {
        }

        public DatumResponse(String owner, String key, String layer, String path, long timestamp, String s, Dictionary<String, String> meta, Object a, String id, String rev)
            : this(owner, key, layer, path, timestamp, s, meta, a, id, rev, default(TData))
        {
        }

        public DatumResponse(String owner, String key, String layer, String path, long timestamp, String s, Dictionary<String, String> meta, Object a, String id, String rev, TData data)
        {
            this.Owner = owner;
            this.Key = key;
            this.Layer = layer;
            this.Path = path;
            this.Timestamp = timestamp;
            this.S = s;
            this.Meta = meta;
            this.A = a;
            this.Id = id;
            this.Rev = rev;
            this.Data = data;
        }

        public static DatumResponse<TData> CopyAndFormatData<TInputData>(DatumResponse<TInputData> copyDatum, Func<TInputData, TData> formatInputDataFunc)
        {
            return new DatumResponse<TData>(copyDatum.Owner, copyDatum.Key, copyDatum.Layer, copyDatum.Path, copyDatum.Timestamp, 
                copyDatum.S, copyDatum.Meta, copyDatum.A, copyDatum.Id, copyDatum.Rev, formatInputDataFunc(copyDatum.Data));
        }

        protected bool Equals(DatumResponse<TData> other)
        {
            return String.Equals(Owner, other.Owner) && String.Equals(Key, other.Key) && String.Equals(Layer, other.Layer) && String.Equals(Path, other.Path) && 
                   Timestamp == other.Timestamp && Equals(Meta, other.Meta) && EqualityComparer<TData>.Default.Equals(Data, other.Data) && 
                   String.Equals(S, other.S) && Equals(A, other.A) && String.Equals(Id, other.Id) && String.Equals(Rev, other.Rev);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DatumResponse<TData>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Owner != null ? Owner.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Key != null ? Key.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Layer != null ? Layer.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Path != null ? Path.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Timestamp.GetHashCode();
                hashCode = (hashCode * 397) ^ (Meta != null ? Meta.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ EqualityComparer<TData>.Default.GetHashCode(Data);
                hashCode = (hashCode * 397) ^ (S != null ? S.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (A != null ? A.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Id != null ? Id.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Rev != null ? Rev.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}

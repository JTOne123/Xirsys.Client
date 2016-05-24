using System;

namespace Xirsys.Client.Models.REST
{
    public class TopicData
    {
        public String Topic { get; set; }

        public String Service { get; set; }

        public String Path { get; set; }

        public String Origin { get; set; }

        public String Meta { get; set; }

        public String Key { get; set; }

        public String Ident { get; set; }

        public TopicData()
        {
        }

        public TopicData(String topic, String service, String path, String origin, String meta, String key, String ident)
        {
            this.Topic = topic;
            this.Service = service;
            this.Path = path;
            this.Origin = origin;
            this.Meta = meta;
            this.Key = key;
            this.Ident = ident;
        }

        public TopicData(TopicData other)
            : this(other.Topic, other.Service, other.Path, other.Origin, other.Meta, other.Key, other.Ident)
        {
        }

        protected Boolean Equals(TopicData other)
        {
            return String.Equals(Topic, other.Topic) && String.Equals(Service, other.Service) && String.Equals(Path, other.Path) && String.Equals(Origin, other.Origin) && String.Equals(Meta, other.Meta) && String.Equals(Key, other.Key) && String.Equals(Ident, other.Ident);
        }

        public override Boolean Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TopicData) obj);
        }

        public override Int32 GetHashCode()
        {
            unchecked
            {
                Int32 hashCode = (Topic != null ? Topic.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Service != null ? Service.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Path != null ? Path.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Origin != null ? Origin.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Meta != null ? Meta.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Key != null ? Key.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Ident != null ? Ident.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static Boolean operator ==(TopicData left, TopicData right)
        {
            return Equals(left, right);
        }

        public static Boolean operator !=(TopicData left, TopicData right)
        {
            return !Equals(left, right);
        }
    }
}

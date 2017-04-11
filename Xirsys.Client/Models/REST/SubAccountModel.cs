using System;
using System.Runtime.Serialization;

namespace Xirsys.Client.Models.REST
{
    [DataContract]
    public class SubAccountModel : BaseSubAccountModel
    {
        [DataMember(Name = "username")]
        public String UserName { get; set; }

        [DataMember(Name = "active")]
        public Boolean Active { get; set; }

        // unix timestamp in UTC timezone
        [DataMember(Name = "created")]
        public Int32 Created { get; set; }

        public SubAccountModel()
        {
        }

        public SubAccountModel(SubAccountModel other)
            : base((BaseSubAccountModel)other)
        {
            this.UserName = other.UserName;
            this.Active = other.Active;
            this.Created = other.Created;
        }



        protected Boolean Equals(SubAccountModel other)
        {
            return base.Equals(other) && String.Equals(UserName, other.UserName) && Active == other.Active && Created == other.Created;
        }

        public override Boolean Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SubAccountModel) obj);
        }

        public override Int32 GetHashCode()
        {
            unchecked
            {
                Int32 hashCode = base.GetHashCode();
                hashCode = (hashCode*397) ^ (UserName != null ? UserName.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ Active.GetHashCode();
                hashCode = (hashCode*397) ^ Created;
                return hashCode;
            }
        }

        public static Boolean operator ==(SubAccountModel left, SubAccountModel right)
        {
            return Equals(left, right);
        }

        public static Boolean operator !=(SubAccountModel left, SubAccountModel right)
        {
            return !Equals(left, right);
        }
    }
}

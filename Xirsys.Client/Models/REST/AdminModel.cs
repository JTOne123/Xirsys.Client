using System;
using System.Runtime.Serialization;

namespace Xirsys.Client.Models.REST
{
    [DataContract]
    public class AdminModel : BaseAdminModel
    {
        [IgnoreDataMember]
        public String UserName { get; set; }

        // unix timestamp in UTC timezone
        [DataMember(Name = "created")]
        public Int32 Created { get; set; }

        public AdminModel()
        {
        }

        public AdminModel(String userName, String email, String password)
        {
            this.UserName = userName;
            this.Email = email;
            this.Password = password;
        }

        public AdminModel(AdminModel other)
            : base((BaseAdminModel)other)
        {
            this.UserName = other.UserName;
            this.Created = other.Created;
        }




        protected Boolean Equals(AdminModel other)
        {
            return base.Equals(other) && String.Equals(UserName, other.UserName) && Created == other.Created;
        }

        public override Boolean Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AdminModel) obj);
        }

        public override Int32 GetHashCode()
        {
            unchecked
            {
                Int32 hashCode = base.GetHashCode();
                hashCode = (hashCode*397) ^ (UserName != null ? UserName.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ Created;
                return hashCode;
            }
        }

        public static Boolean operator ==(AdminModel left, AdminModel right)
        {
            return Equals(left, right);
        }

        public static Boolean operator !=(AdminModel left, AdminModel right)
        {
            return !Equals(left, right);
        }
    }
}

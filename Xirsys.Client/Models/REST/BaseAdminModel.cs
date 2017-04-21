using System;
using System.Runtime.Serialization;

namespace Xirsys.Client.Models.REST
{
    [DataContract]
    public class BaseAdminModel
    {
        public const int MIN_REQ_PASSWORD_LENGTH = 4;
        public const String DEFAULT_ROLE = "admin";

        [DataMember(Name = "email")]
        public String Email { get; set; }

        [DataMember(Name = "password")]
        public String Password { get; set; }

        [DataMember(Name = "first_name")]
        public String FirstName { get; set; }

        [DataMember(Name = "last_name")]
        public String LastName { get; set; }

        //[DataMember(Name = "company")]
        //public String Company { get; set; }

        [DataMember(Name = "role")]
        public String Role { get; set; }
        
        public BaseAdminModel()
        {
        }

        public BaseAdminModel(String email, String password)
            : this(email, password, null, null, null)
        {
        }

        public BaseAdminModel(String email, String password, String firstName, String lastName, String role)
        {
            this.Email = email;
            this.Password = password;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Role = role;
        }

        public BaseAdminModel(BaseAdminModel other)
        {
            this.Email = other.Email;
            this.Password = other.Password;

            this.FirstName = other.FirstName;
            this.LastName = other.LastName;
            //this.Company = other.Company;

            this.Role = other.Role;
        }


        protected Boolean Equals(BaseAdminModel other)
        {
            return String.Equals(Email, other.Email) && String.Equals(Password, other.Password) && String.Equals(FirstName, other.FirstName) && String.Equals(LastName, other.LastName) /*&& String.Equals(Company, other.Company)*/ && String.Equals(Role, other.Role);
        }

        public override Boolean Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BaseAdminModel) obj);
        }

        public override Int32 GetHashCode()
        {
            unchecked
            {
                Int32 hashCode = (Email != null ? Email.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Password != null ? Password.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (FirstName != null ? FirstName.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (LastName != null ? LastName.GetHashCode() : 0);
                //hashCode = (hashCode*397) ^ (Company != null ? Company.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Role != null ? Role.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static Boolean operator ==(BaseAdminModel left, BaseAdminModel right)
        {
            return Equals(left, right);
        }

        public static Boolean operator !=(BaseAdminModel left, BaseAdminModel right)
        {
            return !Equals(left, right);
        }
    }
}

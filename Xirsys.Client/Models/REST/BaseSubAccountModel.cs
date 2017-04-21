using System;
using System.Runtime.Serialization;

namespace Xirsys.Client.Models.REST
{
    [DataContract]
    public class BaseSubAccountModel
    {
        [DataMember(Name = "email")]
        public String Email { get; set; }

        [DataMember(Name = "secret")]
        public String Secret { get; set; }

        [DataMember(Name = "first_name")]
        public String FirstName { get; set; }

        [DataMember(Name = "last_name")]
        public String LastName { get; set; }

        [DataMember(Name = "company")]
        public String Company { get; set; }

        // present in all accounts, but not exactly valid for sub-accounts afaik

        //[DataMember(Name = "customer_id")]
        //public String CustomerId { get; set; }

        //[DataMember(Name = "password")]
        //public String Password { get; set; }

        //[DataMember(Name = "term")]
        //public String Term { get; set; }

        //[DataMember(Name = "plan")]
        //public String Plan { get; set; }


        public BaseSubAccountModel()
        {
        }

        public BaseSubAccountModel(String email)
            : this(email, null, null, null, null)
        {
        }

        public BaseSubAccountModel(String email, String secret)
            : this(email, secret, null, null, null)
        {
        }

        public BaseSubAccountModel(String email, String secret, String firstName, String lastName, String company)
        {
            this.Email = email;
            this.Secret = secret;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Company = company;
        }

        public BaseSubAccountModel(BaseSubAccountModel other)
        {
            this.Email = other.Email;

            this.FirstName = other.FirstName;
            this.LastName = other.LastName;
            this.Company = other.Company;

            this.Secret = other.Secret;
        }

        public virtual void MergeUpdateModel(UpdateSubAccountModel updateSubAccount)
        {
            if (updateSubAccount.Email != null)
            {
                this.Email = updateSubAccount.Email;
            }

            if (updateSubAccount.FirstName != null)
            {
                this.FirstName = updateSubAccount.FirstName;
            }
            if (updateSubAccount.LastName != null)
            {
                this.LastName = updateSubAccount.LastName;
            }
            if (updateSubAccount.Company != null)
            {
                this.Company = updateSubAccount.Company;
            }

            if (updateSubAccount.Secret != null)
            {
                this.Secret = updateSubAccount.Secret;
            }
        }




        protected Boolean Equals(BaseSubAccountModel other)
        {
            return String.Equals(Email, other.Email) && String.Equals(Secret, other.Secret) && String.Equals(FirstName, other.FirstName) && String.Equals(LastName, other.LastName) && String.Equals(Company, other.Company);
        }

        public override Boolean Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BaseSubAccountModel) obj);
        }

        public override Int32 GetHashCode()
        {
            unchecked
            {
                Int32 hashCode = (Email != null ? Email.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Secret != null ? Secret.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (FirstName != null ? FirstName.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (LastName != null ? LastName.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Company != null ? Company.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static Boolean operator ==(BaseSubAccountModel left, BaseSubAccountModel right)
        {
            return Equals(left, right);
        }

        public static Boolean operator !=(BaseSubAccountModel left, BaseSubAccountModel right)
        {
            return !Equals(left, right);
        }
    }
}

using System;
using System.Runtime.Serialization;

namespace Xirsys.Client.Models.REST
{
    [DataContract]
    public class UpdateSubAccountModel : BaseSubAccountModel
    {
        [DataMember(Name = "active")]
        public Nullable<Boolean> Active { get; set; }

        public UpdateSubAccountModel()
        {
        }

        public UpdateSubAccountModel(UpdateSubAccountModel other) 
            : base((BaseSubAccountModel)other)
        {
            this.Active = other.Active;
        }
    }
}

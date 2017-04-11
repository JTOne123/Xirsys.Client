using System;
using System.Runtime.Serialization;

namespace Xirsys.Client.Models.REST
{
    [DataContract]
    public class UpdateSubAccountModel : BaseSubAccountModel
    {
        [DataMember(Name = "active")]
        public Nullable<Boolean> Active { get; set; }

        // unix timestamp in UTC timezone
        [DataMember(Name = "created")]
        public Nullable<Int32> Created { get; set; }

        public UpdateSubAccountModel()
        {
        }

        public UpdateSubAccountModel(UpdateSubAccountModel other) 
            : base((BaseSubAccountModel)other)
        {
            this.Active = other.Active;
            this.Created = other.Created;
        }
    }
}

using System;
using System.Runtime.Serialization;

namespace Xirsys.Client.Models.REST
{
    [DataContract]
    public class XirsysResponseModel<TData>
    {
        [DataMember(Name = "s")]
        public String Status { get; set; }

        [DataMember(Name = "v")]
        public TData Data { get; set; }

        // Xirsys unfortunately uses the "v" property for both successful response data and error messages
        // in our object we move any errors to this property instead
        [DataMember(Name = "error_reason", IsRequired = false)]
        public String ErrorResponse { get; set; }

        public XirsysResponseModel()
        {
        }

        public XirsysResponseModel(String status, TData data)
        {
            this.Status = status;
            this.Data = data;
        }

        public XirsysResponseModel(String status, String errorResponse, TData data)
        {
            this.Status = status;
            this.ErrorResponse = errorResponse;
            this.Data = data;
        }

        public XirsysResponseModel(XirsysResponseModel<TData> other)
        {
            this.Status = other.Status;
            this.Data = other.Data;
            this.ErrorResponse = other.ErrorResponse;
        }
    }
}

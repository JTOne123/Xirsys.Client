using System;
using System.Collections.Generic;
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

        [IgnoreDataMember]
        public String RawHttpResponse { get; set; }

        public XirsysResponseModel()
        {
        }

        public XirsysResponseModel(String status, TData data, String rawHttpResponse)
            : this(status, null, data, rawHttpResponse)
        {
        }

        public XirsysResponseModel(String status, String errorResponse, TData data, String rawHttpResponse)
        {
            this.Status = status;
            this.ErrorResponse = errorResponse;
            this.Data = data;
            this.RawHttpResponse = rawHttpResponse;
        }

        public XirsysResponseModel(XirsysResponseModel<TData> other)
            : this(other.Status, other.ErrorResponse, other.Data, other.RawHttpResponse)
        {
        }

        protected Boolean Equals(XirsysResponseModel<TData> other)
        {
            return String.Equals(Status, other.Status) && EqualityComparer<TData>.Default.Equals(Data, other.Data) && String.Equals(ErrorResponse, other.ErrorResponse);
        }

        public override Boolean Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((XirsysResponseModel<TData>) obj);
        }

        public override Int32 GetHashCode()
        {
            unchecked
            {
                Int32 hashCode = (Status != null ? Status.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ EqualityComparer<TData>.Default.GetHashCode(Data);
                hashCode = (hashCode*397) ^ (ErrorResponse != null ? ErrorResponse.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static Boolean operator ==(XirsysResponseModel<TData> left, XirsysResponseModel<TData> right)
        {
            return Equals(left, right);
        }

        public static Boolean operator !=(XirsysResponseModel<TData> left, XirsysResponseModel<TData> right)
        {
            return !Equals(left, right);
        }
    }
}

using System;
using System.Runtime.Serialization;

namespace VolatilityContracts
{
    [DataContract]
    public class CustomerDetails:Customer
    {
        
        [DataMember]
        public string OtherNames { get; set; }

        [DataMember]
        public Gender Gender { get; set; }

        [DataMember]
        public Title Title { get; set; }

        [DataMember]
        public string ContactNumber { get; set; }

        [DataMember]
        public string EmailAddress { get; set; }

        [DataMember]
        public string PostalAddressLine1 { get; set; }

        [DataMember]
        public string PostalAddressLine2 { get; set; }

        [DataMember]
        public string PostCode { get; set; }

    }
}

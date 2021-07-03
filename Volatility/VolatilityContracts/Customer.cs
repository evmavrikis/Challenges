using System;
using System.Runtime.Serialization;

namespace VolatilityContracts
{
    [DataContract]
    public class Customer
    {
        
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public DateTime DOB { get; set; } = DateTime.Now;

    }
}

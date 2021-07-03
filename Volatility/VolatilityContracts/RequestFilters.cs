
using System.Runtime.Serialization;

namespace VolatilityContracts
{
    [DataContract]
    public class RequestFilters
    {
        [DataMember]
        public string FirstName { get; set; } = "";

        [DataMember]
        public string LastName { get; set; } = "";
    }
}

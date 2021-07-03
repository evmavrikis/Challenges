using System;
using System.Runtime.Serialization;

namespace VolatilityContracts
{
    [DataContract(Name = "Gender")]
    public enum  Gender
    {
        [EnumMember]
        Female = 1,
        [EnumMember]
        Male = 2,
        [EnumMember]
        Other = 3,
    }


    public enum Title
    {
        [EnumMember]
        Mr = 1,
        [EnumMember]
        Mrs = 2,
        [EnumMember]
        Ms = 3,
        [EnumMember]
        Dr = 4,
        [EnumMember]
        Sir = 5,
        [EnumMember]
        Professor = 6,
    }

    public enum Notification
    {
        [EnumMember]
        RecordAdded = 10,

        [EnumMember]
        RecordUpdated = 20,

        [EnumMember]
        RecordDeleted = 30,

        [EnumMember]
        UnexpectedError = 40,

    }
}

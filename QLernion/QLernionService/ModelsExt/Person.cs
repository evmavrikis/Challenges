using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLernionService.Models
{
    public partial class Person
    {
        public override bool Equals(object obj)
        {
            return Equals(obj as Person);
        }

        public bool Equals(Person other)
        {
            return other != null &&
                   Id == other.Id &&
                   FirstName == other.FirstName &&
                   LastName == other.LastName &&
                   Dob == other.Dob &&
                   Salutation == Salutation;
        }
    }
}

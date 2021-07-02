using System;
using System.Collections.Generic;

#nullable disable

namespace QLernionService.Models
{
    public partial class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Dob { get; set; }
        public string Salutation { get; set; }
    }
}

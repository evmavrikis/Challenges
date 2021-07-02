using Microsoft.VisualStudio.TestTools.UnitTesting;
using QLernionService.Models;
using QLernionService.Controllers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;


namespace QLernionControllerTests
{
    [TestClass]
    public class LowLevelTests
    {
        [TestMethod]
        public void InsertRetrieveSinglePerson()
        {    
            var c = new PersonController(false);         
            var p = new Person()
            {
                Id = 0,
                FirstName = "Albert",
                LastName = "Einstein",
                Dob = new DateTime(1879, 3, 14),
                Salutation = "Dear Prof"
            };
            c.Post(p);

            var ps = c.Get();
            var p2 = ps.First();
            p.Id = p2.Id;
            Assert.IsTrue (p2.Equals(p));
        }

        [TestMethod]
        public void InsertRetrieveMultiplePersons()
        {

            var c = new PersonController(false);

            var p = new Person()
            {
                Id = 0,
                FirstName = "Albert",
                LastName = "Einstein",
                Dob = new DateTime(1879, 3, 14),
                Salutation = "Dear Prof"
            };
            c.Post(p);

            var p1 = new Person()
            {
                Id = 0,
                FirstName = "George",
                LastName = "Best",
                Dob = new DateTime(1946, 5, 22),
                Salutation = "Georgey"
            };
            c.Post(p1);

            var ps = c.Get();

            Assert.IsTrue(ps.Count() == 2);
        }

        [TestMethod]
        public void RetrieveMissingPerson()
        {

            var c = new PersonController(false);
            var ps = c.Get(8);

            Assert.IsTrue(ps == null);
        }

        [TestMethod]
        public void UpdatePerson()
        {
            var c = new PersonController(false);

            var p = new Person()
            {
                Id = 0,
                FirstName = "Albert",
                LastName = "Einstein",
                Dob = new DateTime(1879, 3, 14),
                Salutation = "Dear Prof"
            };
            c.Post(p);
            var p2 = c.Get().First();
            p2.Salutation = "Dear Professor";
            c.Put(p2.Id, p);
            var p3 = c.Get().First();
            Assert.IsTrue(p2.Salutation == p3.Salutation);
        }

        
        [TestInitialize]
        public void CleanUp()
        {
            var c = new PersonController(false);
            var ps = c.Get();
            foreach (var p in ps)
            {
                c.Delete(p.Id);
            }
        }
            
            
    }

    
}

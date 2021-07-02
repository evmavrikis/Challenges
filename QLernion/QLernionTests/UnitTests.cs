using Microsoft.VisualStudio.TestTools.UnitTesting;
using QLernionService.Models;
using QLernionService.Controllers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace QLernionControllerTests
{
    [TestClass]
    public class UnitTests
    {
        private static QLernionDBConext _dbContext;

        
        [ClassInitialize()]
        public static void SetUp(TestContext testContext)
        {
            var options = new DbContextOptionsBuilder<QLernionDBConext>()
                   .UseInMemoryDatabase(databaseName: "QLernionDB")
                   .Options;
            _dbContext = new QLernionDBConext(options);

            
        }

        [TestInitialize]
        public void RemoveRecords()
        {
            _dbContext.Dispose();
            var options = new DbContextOptionsBuilder<QLernionDBConext>()
                   .UseInMemoryDatabase(databaseName: "QLernionDB")
                   .Options;
            _dbContext = new QLernionDBConext(options);
            var people = _dbContext.People;
            _dbContext.RemoveRange(people);

            
        }
        [TestMethod]    
        public void InsertRetrieveSinglePerson()
        {
 
            var c = new PersonController(_dbContext);
            
            var p = new Person()
            {
                Id = 3,
                FirstName = "Albert",
                LastName = "Einstein",
                Dob = new DateTime(1879, 3, 14),
                Salutation = "Dear Prof"
            };
            c.Post(p);

            var p2 = c.Get(3);

            Assert.IsTrue (p2.Equals(p));
        }

        [TestMethod]
        public void InsertRetrieveMultiplePersons()
        {
            var c = new PersonController(_dbContext);

            var p = new Person()
            {
                Id = 3,
                FirstName = "Albert",
                LastName = "Einstein",
                Dob = new DateTime(1879, 3, 14),
                Salutation = "Dear Prof"
            };
            c.Post(p);

            var p1 = new Person()
            {
                Id = 4,
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
        public void InsertDeletePerson()
        {
           
            var c = new PersonController(_dbContext);

            var p = new Person()
            {
                Id = 3,
                FirstName = "Albert",
                LastName = "Einstein",
                Dob = new DateTime(1879, 3, 14),
                Salutation = "Dear Prof"
            };
            c.Post(p);

            var p1 = new Person()
            {
                Id = 4,
                FirstName = "George",
                LastName = "Best",
                Dob = new DateTime(1946, 5, 22),
                Salutation = "Georgey"
            };
            c.Post(p1);

            c.Delete(4);
            var ps = c.Get();

            Assert.IsTrue(ps.Count() == 1 && ps.First().Equals(p));
        }

        [TestMethod]
        public void UpdatePerson()
        {
            var c = new PersonController(_dbContext);

            var p = new Person()
            {
                Id = 3,
                FirstName = "Albert",
                LastName = "Einstein",
                Dob = new DateTime(1879, 3, 14),
                Salutation = "Dear Prof"
            };
            c.Post(p);

            p.Salutation = "Dear Professor";
            c.Put(3, p);

            var p2 = c.Get(3);         
            Assert.IsTrue(p2.Salutation == "Dear Professor");
        }


        [TestMethod]
        public void RetrieveMissingPerson()
        {

            var c = new PersonController(_dbContext);
            var ps = c.Get(8);

            Assert.IsTrue(ps == null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException) )]
        public void DublicateRecord()
        {
            var c = new PersonController(_dbContext);

            var p = new Person()
            {
                Id = 3,
                FirstName = "Albert",
                LastName = "Einstein",
                Dob = new DateTime(1879, 3, 14),
                Salutation = "Dear Prof"
            };
            c.Post(p);
            c.Post(p);

        }

        [TestMethod]
        [ExpectedException(typeof(DbUpdateConcurrencyException))]
        public void UpdateMissing()
        {
            var c = new PersonController(_dbContext);

            var p = new Person()
            {
                Id = 3,
                FirstName = "Albert",
                LastName = "Einstein",
                Dob = new DateTime(1879, 3, 14),
                Salutation = "Dear Prof"
            };
            c.Put(1, p);
           
        }

    }
}

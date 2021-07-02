using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Collections.Generic;
using System;
using System.Linq;

namespace QLernionSystemTests
{
   

    /// <summary>
    /// Basic systems tests for in memory mode.
    /// The service must have been started in the in memory mode (json settings config).
    /// </summary>
    [TestClass]
    public class InMemoryDBSystemTests
    {
        private const string LOCAL_URL = "http://localhost:1280/person";
        private const string AZURE_URL = "https://qlernionservice01.azurewebsites.net/person";

        private const string TOKEN = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1lIjoiRXZhbmdlbG9" +
            "zIE1hdnJpa2lzIiwiZW1haWwiOiJlX21hdnJpa2lzQGhvdG1haWwuY29tIiwiTnVtYmVyT2ZNb250aHMiOiI2Ny" +
            "IsImV4cCI6MTYyMzg1NzU4MywiaXNzIjoiUXVhcmsiLCJhdWQiOiJRdWFyayJ9.hxebe3bWUYlEUN-8riE3tNA7HzEPDFhvIzUL9wWvzOE";

        private const string TOKEN_WRONG = "eyJhbGciOiJIUzI1GiIsInR5cCI6IkpXVCJ9.eyJuYW1lIjoiRXZhbmdlbG9" +
            "zIE1hdnJca2lzIiwiZW1haWEiOiJlX21hdnJpa2lzQGhvdG1haWwuY29tIiwiDnVtYmVyT2ZNb250aHMiOiI2Ny" +
            "IsImV4cCI6MTYyMzg1NzU4MywiaXNzIjoiUXVhcmsiLCJhdWQiOiJRdWFyayJ9.hxebe3bWUYlEUN-8riE3tNA7HzEPDFhvIzUL9wWvzOE";
        private readonly HttpClient _client = new HttpClient();
        
        
        private string _url = AZURE_URL;

        [TestInitialize]
        public async Task DoCleanUp()
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + TOKEN);
            await CleanUp();
        }

        [TestMethod]
        public async Task insertRetrieveSingle()
        {
            try
            {
                
                var person = new Person()
                {
                    Id = 1,
                    FirstName = "Albert",
                    LastName = "Einstein",
                    Dob = new DateTime(1879, 3, 14),
                    Salutation = "Dear Prof"
                };
                StringContent cont = new StringContent(JsonConvert.SerializeObject(person), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _client.PostAsync(_url, cont);
                response.EnsureSuccessStatusCode();
                HttpResponseMessage response2 = await _client.GetAsync(_url);
                response2.EnsureSuccessStatusCode();
                string responseBody2 = await response2.Content.ReadAsStringAsync();
                var people = JsonConvert.DeserializeObject<List<Person>>(responseBody2);

                HttpResponseMessage response5 = await _client.GetAsync( _url + "/1" );
                response5.EnsureSuccessStatusCode();
                string responseBody5 = await response5.Content.ReadAsStringAsync();
                var p5 = JsonConvert.DeserializeObject<Person>(responseBody5);

                Assert.IsTrue(people.Count == 1 && people.First().Id == 1 && p5.Id == 1);
                


            }
            catch
            {
               
                Assert.IsTrue(false);
            }
        }

        [TestMethod]
        public async Task InsertRetrieveModify()
        {
            try
            {

                var person = new Person()
                {
                    Id = 1,
                    FirstName = "Albert",
                    LastName = "Einstein",
                    Dob = new DateTime(1879, 3, 14),
                    Salutation = "Dear Prof"
                };
                StringContent cont = new StringContent(JsonConvert.SerializeObject(person), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _client.PostAsync(_url, cont);
                response.EnsureSuccessStatusCode();

                var person2 = new Person()
                {
                    Id = 2,
                    FirstName = "George",
                    LastName = "Best",
                    Dob = new DateTime(1946, 5, 22),
                    Salutation = "Georgey"
                };
                StringContent cont2 = new StringContent(JsonConvert.SerializeObject(person2), Encoding.UTF8, "application/json");
                HttpResponseMessage response2 = await _client.PostAsync(_url, cont2);
                response2.EnsureSuccessStatusCode();

                HttpResponseMessage response3 = await _client.GetAsync(_url);
                response3.EnsureSuccessStatusCode();
                string responseBody3 = await response3.Content.ReadAsStringAsync();

                var people = JsonConvert.DeserializeObject<List<Person>>(responseBody3);

                person2.Salutation = "Hey Matey!";
                StringContent cont4 = new StringContent(JsonConvert.SerializeObject(person2), Encoding.UTF8, "application/json");
                HttpResponseMessage response4 = await _client.PutAsync(string.Format(_url +"/{0}", person2.Id), cont4);

                Assert.IsTrue(people.Count == 2);
                

            }
            catch
            {
                
                Assert.IsTrue(false);
            }
        }
        [TestMethod]
        public async Task UpdateMissingRecord()
        {
            var person = new Person()
            {
                Id = 1,
                FirstName = "Albert",
                LastName = "Einstein",
                Dob = new DateTime(1879, 3, 14),
                Salutation = "Dear Prof"
            };
            StringContent cont = new StringContent(JsonConvert.SerializeObject(person), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PutAsync(string.Format(_url +"/{0}", person.Id), cont);
            Assert.IsFalse(response.IsSuccessStatusCode);
            
           
        }
        private async Task CleanUp()
        {
            HttpResponseMessage response = await _client.GetAsync(_url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            var people = JsonConvert.DeserializeObject<List<Person>>(responseBody);
            
            foreach (var p in people)
            {
                response = await _client.DeleteAsync(string.Format(_url + "/{0}", p.Id));


            }

        }
        [TestMethod]
        public async Task AuthFailGet()
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + TOKEN_WRONG);
            HttpResponseMessage response = await _client.GetAsync(_url);

            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.Unauthorized);
        }

        [TestMethod]
        public async Task AuthFailGetId()
        {
            
            var person = new Person()
            {
                Id = 1,
                FirstName = "Albert",
                LastName = "Einstein",
                Dob = new DateTime(1879, 3, 14),
                Salutation = "Dear Prof"
            };
            StringContent cont = new StringContent(JsonConvert.SerializeObject(person), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync(_url, cont);
            response.EnsureSuccessStatusCode();

            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + TOKEN_WRONG);

            HttpResponseMessage response5 = await _client.GetAsync(_url + "/1");
            string responseBody5 = await response5.Content.ReadAsStringAsync();
            var p5 = JsonConvert.DeserializeObject<Person>(responseBody5);

            Assert.IsTrue(response5.StatusCode == System.Net.HttpStatusCode.Unauthorized);

        }

        [TestMethod]
        public async Task AuthFailPost()
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + TOKEN_WRONG);

            var person = new Person()
            {
                Id = 1,
                FirstName = "Albert",
                LastName = "Einstein",
                Dob = new DateTime(1879, 3, 14),
                Salutation = "Dear Prof"
            };
            StringContent cont = new StringContent(JsonConvert.SerializeObject(person), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync(_url, cont);

            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.Unauthorized);
        }

        [TestMethod]
        public async Task AuthFailPut()
        {
            var person = new Person()
            {
                Id = 1,
                FirstName = "Albert",
                LastName = "Einstein",
                Dob = new DateTime(1879, 3, 14),
                Salutation = "Dear Prof"
            };
            StringContent cont = new StringContent(JsonConvert.SerializeObject(person), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync(_url, cont);
            response.EnsureSuccessStatusCode();

            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + TOKEN_WRONG);

            StringContent cont4 = new StringContent(JsonConvert.SerializeObject(person), Encoding.UTF8, "application/json");
            HttpResponseMessage response4 = await _client.PutAsync(string.Format(_url + "/{0}", person.Id), cont4);

            Assert.IsTrue(response4.StatusCode == System.Net.HttpStatusCode.Unauthorized);
        }

        [TestMethod]
        public async Task AuthFailDelete()
        {
            var person = new Person()
            {
                Id = 1,
                FirstName = "Albert",
                LastName = "Einstein",
                Dob = new DateTime(1879, 3, 14),
                Salutation = "Dear Prof"
            };
            StringContent cont = new StringContent(JsonConvert.SerializeObject(person), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync(_url, cont);
            response.EnsureSuccessStatusCode();

            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + TOKEN_WRONG);

            
            HttpResponseMessage response4 = await _client.DeleteAsync(string.Format(_url + "/{0}", person.Id));

            Assert.IsTrue(response4.StatusCode == System.Net.HttpStatusCode.Unauthorized);
        }
    }
}

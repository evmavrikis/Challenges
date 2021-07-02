using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
namespace QLernionSystemTests
{
    [TestClass]
    public class InMemoryDBMassTest
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

        [TestMethod]
        public async Task FireAtRandomWill()
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + TOKEN);

            int nTests = 200;
            HashSet<int> usedIds = new HashSet<int>();
            Task<HttpResponseMessage> [] tasks = new Task<HttpResponseMessage>[nTests];
            int count = 0;
            Random rnd = new Random(5);

            DateTime start = DateTime.Now;
            while (count < tasks.Length)
            {
                if ((DateTime.Now - start).TotalMilliseconds % 10 == 0)
                {
                    var r =   rnd.Next(0, 10);
                    for (int i=1; i<=r;i++)
                    {
                        var ch = rnd.Next(1, 10);
                        if (ch <=2)
                        {
                            var id = rnd.Next(1, 10000);

                            if (!usedIds.Contains(id))
                            {
                                var person = new Person()
                                {
                                    Id = id,
                                    FirstName = "Albert",
                                    LastName = "Einstein",
                                    Dob = new DateTime(1879, 3, 14),
                                    Salutation = "Dear Prof"
                                };
                                StringContent cont = new StringContent(JsonConvert.SerializeObject(person), Encoding.UTF8, "application/json");
                                
                                tasks[count] = _client.PostAsync(_url, cont);
                                usedIds.Add(id);
                                count++;
                            }
                        }
                        else if (ch <=7)
                        {
                            tasks[count] = _client.GetAsync(_url);
                            count++;
                        }
                        else
                        {
                            if (usedIds.Count > 0)
                            {
                                var id2 = usedIds.First();
                                usedIds.Remove(id2);
                                tasks[count] = _client.DeleteAsync(string.Format(_url + "/{0}", id2));
                                count++;
                            }
                        }
                       

                        
                        if (count >= tasks.Length)
                        {
                            break;
                        }
                    }
                }
            }

            await Task.WhenAll(tasks);
            var c = tasks.Count(t => t.Result.IsSuccessStatusCode);
            Assert.IsTrue (c > nTests * 0.5);

        }
    }
}

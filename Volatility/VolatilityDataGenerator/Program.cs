using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VolatilityContracts;
using System.IO;
using Newtonsoft.Json;

namespace VolatilityDataGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            Generate(10000, "Customers");
        }

        static void Generate(int numOfCustomers, string dataFile)
        {
            List<CustomerDetails> customers = new List<CustomerDetails>();
            var femaleNames = File.ReadAllLines("FemaleNames.txt");
            var maleNames = File.ReadAllLines("MaleNames.txt");
            var addresses = File.ReadAllLines("Addresses.txt");
            Random rnd = new Random(1);
            for (int i=1; i<= numOfCustomers;i++)
            {
                var c = new CustomerDetails()
                {
                    Id = i,
                    DOB = new DateTime(rnd.Next(1955, 2004), rnd.Next(1, 13), rnd.Next(1, 28)),
                };

                customers.Add(c);

                var hg = rnd.NextDouble();

                string name = null;
                if (hg < 0.48)
                {
                    c.Gender = Gender.Male;
                    var ni = rnd.Next(0, maleNames.Length);
                    name = maleNames[ni];
                    
                    c.Title = VolatilityContracts.Title.Mr;
                }
                else
                {
                    c.Gender = Gender.Female;
                    var mi = rnd.Next(0, femaleNames.Length);
                    name = femaleNames[mi];
                    c.Title = ( i%3 != 0? VolatilityContracts.Title.Mrs : VolatilityContracts.Title.Ms);
                }
                c.FirstName = name.Split(' ')[0];
                c.LastName = name.Split(' ')[1];
                c.EmailAddress = string.Format("{0}.{1}@gmail.com", c.FirstName, c.LastName);
                c.ContactNumber =  "0" +  (700000000 + rnd.Next(0,99999999)).ToString();
                var a = addresses[rnd.Next(0, addresses.Length)].Split(',');
                c.PostCode = a[a.Length-1];
                c.PostalAddressLine1 = a[0] + a[1];
                c.PostalAddressLine1 = c.PostalAddressLine1.Replace("\"", "");
                if (a.Length > 3)
                {
                    for (int j=2; j < a.Length - 1; j++)
                    {
                        c.PostalAddressLine2 += a[j];
                    }
                    
                    c.PostalAddressLine2 = c.PostalAddressLine2.Replace("\"", ""); ;
                }
            }


            var js = JsonConvert.SerializeObject(customers);
            System.IO.File.WriteAllText(dataFile + ".json", js);
        }

      
    }
}

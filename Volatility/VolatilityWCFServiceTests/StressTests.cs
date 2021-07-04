using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VolatilityContracts;
using System.Linq;

namespace VolatilityWCFServiceTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Test1()
        {
            int nClients = 1000;
            int tot = 0;
            List<Client> clients = new List<Client>();
            for (int i=1; i<=nClients; i++)
            {
                var c = new Client();
                clients.Add(c);
                if (c.InitWCFConnection())
                {
                    tot++;
                }
            }

            for (int i = 0; i < nClients; i++)
            {
                clients[i].Context.Close();
            }

            Assert.IsTrue(tot == nClients);

        }


        [TestMethod]
        public void MainTest()
        {
            int nClients = 500;
            int callsPerTenMilisecond = 20;
            int testDurectionSeconds = 5;

            int tot = 0;
            List<Client> clients = new List<Client>();
            for (int i = 1; i <= nClients; i++)
            {
                var c = new Client();
                clients.Add(c);
                if (c.InitWCFConnection())
                {
                    tot++;
                }
            }


            var dtStart = DateTime.Now;
            Random rnd = new Random(1);
            var dt = DateTime.Now;

            var customers = clients[0].Service.GetCustomers(new VolatilityContracts.RequestFilters()).ToList();
            long updates = 0;

            while ((DateTime.Now - dtStart).TotalSeconds < testDurectionSeconds)
            {
                if ((DateTime.Now - dt).TotalMilliseconds >= 10)
                {
                    dt = DateTime.Now;

                    for (int j=1; j<=callsPerTenMilisecond; j++)
                    {
                        var c = clients[rnd.Next(0, clients.Count)];
                        var t = rnd.Next(0, 3);
                        switch (t)
                        {
                            case 0:
                                Task.Run(() => c.Service.GetCustomers(new VolatilityContracts.RequestFilters()));
                                break;
                            case 1:
                                updates++;
                                Task.Run(() => c.Service.DeleteCustomer(-1));
                                break;
                            case 2:
                                Task.Run(
                                    () =>
                                    {
                                        var c1 = customers[rnd.Next(0, customers.Count)];
                                        var cd = c.Service.GetCustomerDetails(c1.Id);
                                        c.Service.UpdateCustomer(cd);
                                    }
                                );
                                updates++;
                                //Task.Run(() => c.Service.UpdateCustomer(cd.Id,));
                                break;
                            case 3:
                                break;
                            case 4:
                                break;

                        }
                         Task.Run(()=> c.Service.GetCustomers(new VolatilityContracts.RequestFilters()));
                    }
                }
            }

            long errors = 0;

            // Loop for a few more seconds
            var start1 = DateTime.Now;

            long updates2 = 0;
           
                
            

            // these will probably timeout.
            for (int i = 0; i < nClients; i++)
            {
                var cl = clients[i];
                //errors += clients[i].Notifications[VolatilityContracts.Notification.UnexpectedError];
                Task.Run(()=> cl.TryClose());
            }

            Thread.Sleep(10000);

            for (int i = 0; i < nClients; i++)
            {
                errors += clients[i].Notifications[VolatilityContracts.Notification.UnexpectedError];
                updates2 += clients[i].Notifications[VolatilityContracts.Notification.RecordUpdated];
                //clients[i].TryClose();
            }

            double perc = 100 * (updates2) / (1.0 * updates * (nClients - 1));
            Assert.IsTrue(tot == nClients && errors == 0 && perc > 5);

        }

        
    }
}


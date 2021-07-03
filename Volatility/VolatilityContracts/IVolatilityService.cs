using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

namespace VolatilityContracts
{
    [ServiceContract(
    SessionMode = SessionMode.Required,
    CallbackContract = typeof(IVolatilityCallback))]
    public interface IVolatilityService
    {
        IEnumerable<Customer> GetCustomers();
        IEnumerable<Customer> GetCustomers(RequestFilters filters);
        CustomerDetails GetCustomerDetails(int Id);
        bool DeleteCustomer(int Id);
        bool UpdateCustomer(CustomerDetails customerDetails);
        bool AddNewCustomer(CustomerDetails customerDetails);

    }
}

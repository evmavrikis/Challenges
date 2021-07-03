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
        [OperationContract]
        IEnumerable<Customer> GetCustomers(RequestFilters filters);

        [OperationContract]
        CustomerDetails GetCustomerDetails(int Id);

        [OperationContract]
        bool DeleteCustomer(int Id);

        [OperationContract]
        bool UpdateCustomer(CustomerDetails customerDetails);

        [OperationContract]
        bool AddNewCustomer(CustomerDetails customerDetails);

    }
}

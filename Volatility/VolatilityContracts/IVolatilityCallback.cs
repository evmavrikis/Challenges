using System.ServiceModel;

namespace VolatilityContracts
{
    public interface IVolatilityCallback
    {
        [OperationContract(IsOneWay = true)]
        void SendNotification(Notification notification);
    }
}

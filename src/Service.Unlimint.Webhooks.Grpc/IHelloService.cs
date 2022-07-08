using System.ServiceModel;
using System.Threading.Tasks;
using Service.Unlimint.Webhooks.Grpc.Models;

namespace Service.Unlimint.Webhooks.Grpc
{
    [ServiceContract]
    public interface IHelloService
    {
        [OperationContract]
        Task<HelloMessage> SayHelloAsync(HelloRequest request);
    }
}
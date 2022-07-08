using System.Runtime.Serialization;
using Service.Unlimint.Webhooks.Domain.Models;

namespace Service.Unlimint.Webhooks.Grpc.Models
{
    [DataContract]
    public class HelloMessage : IHelloMessage
    {
        [DataMember(Order = 1)]
        public string Message { get; set; }
    }
}
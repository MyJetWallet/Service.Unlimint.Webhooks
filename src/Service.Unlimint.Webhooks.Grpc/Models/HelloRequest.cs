using System.Runtime.Serialization;

namespace Service.Unlimint.Webhooks.Grpc.Models
{
    [DataContract]
    public class HelloRequest
    {
        [DataMember(Order = 1)]
        public string Name { get; set; }
    }
}
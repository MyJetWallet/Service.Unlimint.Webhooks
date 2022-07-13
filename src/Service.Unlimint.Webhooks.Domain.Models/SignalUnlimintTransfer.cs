using System.Runtime.Serialization;
using Service.Unlimint.Signer.Grpc.Models;

namespace Service.Unlimint.Webhooks.Domain.Models
{
    [DataContract]
    public class SignalUnlimintTransfer
    {
        public const string ServiceBusTopicName = "unlimint-transfer-signal";

        [DataMember(Order = 1)] public string BrokerId { get; set; }
        [DataMember(Order = 2)] public string ClientId { get; set; }
        [DataMember(Order = 3)] public string WalletId { get; set; }
        [DataMember(Order = 4)] public GetPaymentInfo PaymentInfo { get; set; }
    }
}
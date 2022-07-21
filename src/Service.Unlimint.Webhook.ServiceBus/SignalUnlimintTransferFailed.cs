using MyJetWallet.Unlimint.Models.Payments;
using System.Runtime.Serialization;

namespace Service.Unlimint.Webhook.ServiceBus
{
    [DataContract]
    public class SignalUnlimintTransferFailed
    {
        public const string ServiceBusTopicName = "unlimint-transfer-failed";

        [DataMember(Order = 1)] public string MerchantOrderId { get; set; }

        [DataMember(Order = 2)] public PaymentStatus Status { get; set; }

        [DataMember(Order = 3)] public PaymentErrorCode? DeclineCode { get; set; }

        [DataMember(Order = 4)] public string DeclineReason { get; set; }

    }
}

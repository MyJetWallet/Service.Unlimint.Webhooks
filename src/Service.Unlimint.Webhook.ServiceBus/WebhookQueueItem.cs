using System.Runtime.Serialization;

namespace Service.Unlimint.Webhook.ServiceBus
{
    [DataContract]
    public class WebhookQueueItem
    {
        [DataMember(Order = 1)]
        public string Data { get; set; }
    }
}

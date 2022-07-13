using System;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using Microsoft.Extensions.Logging;
using MyJetWallet.Domain;
using MyJetWallet.Sdk.Service;
using MyJetWallet.Sdk.ServiceBus;
using MyJetWallet.Unlimint.Models.Payments;
using Newtonsoft.Json;
using Service.Unlimint.Signer.Grpc;
using Service.Unlimint.Signer.Grpc.Models;
using Service.Unlimint.Webhook.ServiceBus;
using Service.Unlimint.Webhooks.Domain.Models;
using Service.Unlimint.Webhooks.Services;

namespace Service.Unlimint.Webhooks.Subscribers
{
    public class UnlimintWebhookInternalSubscriber
    {
        private readonly ILogger<UnlimintWebhookInternalSubscriber> _logger;
        private readonly IUnlimintPaymentsService _circlePaymentsService;
        private readonly IServiceBusPublisher<SignalUnlimintTransfer> _transferPublisher;

        public UnlimintWebhookInternalSubscriber(
            ILogger<UnlimintWebhookInternalSubscriber> logger,
            ISubscriber<WebhookQueueItem> subscriber, 
            IUnlimintPaymentsService circlePaymentsService, 
            IServiceBusPublisher<SignalUnlimintTransfer> transferPublisher)
        {
            subscriber.Subscribe(HandleSignal);
            _logger = logger;
            _circlePaymentsService = circlePaymentsService;
            _transferPublisher = transferPublisher;
        }

        private async ValueTask HandleSignal(WebhookQueueItem webhook)
        {
            using var activity = MyTelemetry.StartActivity("Handle Unlimint Event WebhookQueueItem");
            var body = webhook.Data;

            _logger.LogInformation("Processing webhook queue item: {@context}", webhook);

            try
            {
                var dto = JsonConvert.DeserializeObject<WebhookMiddleware.NotificationDto>(body);
                if (dto is { Type: "Notification" })
                {
                    var message = JsonConvert.DeserializeObject<WebhookMiddleware.MessageDto>(dto.Message);
                    if (message != null)
                    {
                        switch (message)
                        {
                            case { NotificationType: "payments" }:
                                {
                                    
                                    break;
                                }
                            case { NotificationType: "transfers" }:
                                {
                                  
                        
                                    break;
                                }
                            case { NotificationType: "cards" }:
                                {
                                   
                                    break;
                                }
                            case { NotificationType: "chargebacks" }:
                                {
                                   
                        
                                    break;
                                }
                            case { NotificationType: "payouts" }:
                                {
                                  
                        
                                    break;
                                }
                            default:
                                _logger.LogInformation("{type} message are not supported", message.NotificationType);
                                break;
                        }
                    }
                    else
                    {
                        _logger.LogInformation("Empty message");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing webhook queue {@context}", webhook);
                ex.FailActivity();
                throw;
            }
        }

        public (string, string, string) ParseDescription(string description)
        {
            if (string.IsNullOrEmpty(description))
                return (null, null, null);

            var prm = description.Split("|-|");

            if (prm.Length != 3)
                return (null, null, null);

            if (string.IsNullOrEmpty(prm[0]) || string.IsNullOrEmpty(prm[1]) || string.IsNullOrEmpty(prm[2]))
                return (null, null, null);

            return (prm[0], prm[1], prm[2]);
        }
    }
}

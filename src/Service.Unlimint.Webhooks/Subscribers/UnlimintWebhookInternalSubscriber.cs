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
        private readonly IUnlimintPaymentsService _unlimintPaymentsService;
        private readonly IServiceBusPublisher<SignalUnlimintTransfer> _transferPublisher;

        public UnlimintWebhookInternalSubscriber(
            ILogger<UnlimintWebhookInternalSubscriber> logger,
            ISubscriber<WebhookQueueItem> subscriber, 
            IUnlimintPaymentsService unlimintPaymentsService, 
            IServiceBusPublisher<SignalUnlimintTransfer> transferPublisher)
        {
            subscriber.Subscribe(HandleSignal);
            _logger = logger;
            _unlimintPaymentsService = unlimintPaymentsService;
            _transferPublisher = transferPublisher;
        }

        private async ValueTask HandleSignal(WebhookQueueItem webhook)
        {
            using var activity = MyTelemetry.StartActivity("Handle Unlimint Event WebhookQueueItem");
            var body = webhook.Data;

            _logger.LogInformation("Processing webhook queue item: {@context}", webhook);

            try
            {
                var callback = JsonConvert.DeserializeObject<PaymentCallback>(body);
                if (callback != null)
                {
                    var paymentData = callback.PaymentData;

                    if (paymentData != null)
                    {
                        
                        var (brokerId, clientId, walletId) = ParseDescription(paymentData.Note);
                        //TODO: Add to payment description BrokerID
                        if (string.IsNullOrEmpty(brokerId))
                        {
                            brokerId = "jetwallet";
                        }
                        var payment = await _unlimintPaymentsService.GetUnlimintPaymentByIdAsync(
                            new GetPaymentByIdRequest
                            {
                                BrokerId = brokerId,
                                PaymentId = callback.PaymentData.Id,
                            });

                        if (payment.Data != null)
                        {
                            _logger.LogInformation("GetUnlimintPaymentByMerchantIdAsync payment info {paymentInfo}",
                                Newtonsoft.Json.JsonConvert.SerializeObject(payment.Data));
                        }

                        if (payment.IsSuccess)
                        {
                            await _transferPublisher.PublishAsync(new SignalUnlimintTransfer()
                            {
                                BrokerId = brokerId,
                                ClientId = clientId,
                                WalletId = walletId,
                                PaymentInfo = payment.Data
                            });
                        }
                        else
                        {
                            _logger.LogError("Unable to get payment info merchantId: {merchantId} paymentId:{paymentId}", 
                                callback.MerchantOrder?.Id, paymentData.Id);
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

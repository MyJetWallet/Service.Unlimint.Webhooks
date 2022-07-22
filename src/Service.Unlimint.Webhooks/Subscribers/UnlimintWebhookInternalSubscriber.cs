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
        private readonly IServiceBusPublisher<SignalUnlimintTransferFailed> _failPublisher;

        public UnlimintWebhookInternalSubscriber(
            ILogger<UnlimintWebhookInternalSubscriber> logger,
            ISubscriber<WebhookQueueItem> subscriber,
            IUnlimintPaymentsService unlimintPaymentsService,
            IServiceBusPublisher<SignalUnlimintTransfer> transferPublisher,
            IServiceBusPublisher<SignalUnlimintTransferFailed> failPublisher)
        {
            subscriber.Subscribe(HandleSignal);
            _logger = logger;
            _unlimintPaymentsService = unlimintPaymentsService;
            _transferPublisher = transferPublisher;
            this._failPublisher = failPublisher;
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

                        if (!string.IsNullOrEmpty(paymentData.Note) && !string.IsNullOrEmpty(callback.PaymentData.Id))
                        {
                            var (brokerId, clientId, walletId) = ParseDescription(paymentData.Note);
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
                                    PaymentInfo = new GetPaymentInfo
                                    {
                                        Id = callback.PaymentData.Id,
                                        Type = payment.Data?.Type,
                                        MerchantOrderId = payment.Data?.MerchantOrderId,
                                        MerchantWalletId = payment.Data?.MerchantWalletId,
                                        Description = payment.Data?.Description,
                                        Status = payment.Data?.Status,
                                        Amount = payment.Data?.Amount,
                                        Fee = payment.Data?.Fee,
                                        Card = payment.Data?.Card,
                                        TrackingRef = payment.Data?.TrackingRef,
                                        ErrorCode = payment.Data?.ErrorCode,
                                        Metadata = payment.Data?.Metadata,
                                        CreateDate = payment.Data?.CreateDate,
                                    }

                                });
                            }
                            else
                            {
                                _logger.LogError("Unable to get payment info callback: {callback}",
                                    webhook.Data);
                                throw new Exception("Retry get payment");
                            }
                        }
                        else
                        {
                            await SendFailedIfMeetConditions(callback);
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

        private async Task SendFailedIfMeetConditions(PaymentCallback callback)
        {
            if (callback.PaymentData.Status == PaymentStatus.Declined ||
                                            callback.PaymentData.Status == PaymentStatus.Cancelled ||
                                            callback.PaymentData.Status == PaymentStatus.Terminated ||
                                            callback.PaymentData.Status == PaymentStatus.Voided)
                await _failPublisher.PublishAsync(new SignalUnlimintTransferFailed()
                {
                    MerchantOrderId = callback.MerchantOrder.Id,
                    DeclineCode = callback.PaymentData?.DeclineCode,
                    DeclineReason = callback.PaymentData?.DeclineReason,
                    Status = callback.PaymentData.Status
                });
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

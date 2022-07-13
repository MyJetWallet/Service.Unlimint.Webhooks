using Autofac;
using MyJetWallet.Sdk.NoSql;
using MyJetWallet.Sdk.ServiceBus;
using Service.Bitgo.DepositDetector.Client;
using Service.Circle.Webhooks;
using Service.Unlimint.Signer.Client;
using Service.Unlimint.Webhook.ServiceBus;
using Service.Unlimint.Webhooks.Domain.Models;
using Service.Unlimint.Webhooks.Subscribers;

namespace Service.Unlimint.Webhooks.Modules
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var serviceBusClient = builder.RegisterMyServiceBusTcpClient(
                Program.ReloadedSettings(e => e.SpotServiceBusHostPort),
                Program.LogFactory);

            builder
                .RegisterMyServiceBusPublisher<SignalUnlimintTransfer>(serviceBusClient,
                    SignalUnlimintTransfer.ServiceBusTopicName, true);

            builder
               .RegisterMyServiceBusPublisher<WebhookQueueItem>(
                   serviceBusClient,
                   Topics.CircleWebhookInternalTopic, 
                   true);

            builder.RegisterMyServiceBusSubscriberSingle<WebhookQueueItem>(
                serviceBusClient,
                Topics.CircleWebhookInternalTopic,
                "service-unlimint-webhook", 
                MyServiceBus.Abstractions.TopicQueueType.Permanent);

            var myNoSqlClient = builder.CreateNoSqlClient(Program.Settings.MyNoSqlReaderHostPort, Program.LogFactory);
            
            builder.RegisterUnlimintPaymentsClient(Program.Settings.UnlimintSignerGrpcServiceUrl);
            builder.RegisterBitgoDepositAddressClient(Program.Settings.BitgoDepositServiceGrpcUrl, myNoSqlClient);

            builder
                .RegisterType<UnlimintWebhookInternalSubscriber>()
                .SingleInstance()
                .AutoActivate();
        }
    }
}
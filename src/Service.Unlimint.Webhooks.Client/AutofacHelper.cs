using Autofac;
using DotNetCoreDecorators;
using MyServiceBus.Abstractions;
using MyServiceBus.TcpClient;
using Service.Unlimint.Webhooks.Domain.Models;

// ReSharper disable UnusedMember.Global

namespace Service.Unlimint.Webhooks.Client
{
    public static class AutofacHelper
    {
        public static void RegisterSignalUnlimintTransferSubscriber(this ContainerBuilder builder,
            MyServiceBusTcpClient client,
            string queueName,
            TopicQueueType queryType)
        {
            var subs = new SignalUnlimintTransferSubscriber(client, queueName, queryType);

            builder
                .RegisterInstance(subs)
                .As<ISubscriber<SignalUnlimintTransfer>>()
                .SingleInstance();
        }
    }
}
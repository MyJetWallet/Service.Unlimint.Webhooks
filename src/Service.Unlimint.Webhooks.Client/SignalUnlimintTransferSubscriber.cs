using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using MyServiceBus.Abstractions;
using MyServiceBus.TcpClient;
using Service.Unlimint.Webhooks.Domain.Models;
using SimpleTrading.ServiceBus.CommonUtils.Serializers;

namespace Service.Unlimint.Webhooks.Client
{
    public class SignalUnlimintTransferSubscriber : ISubscriber<SignalUnlimintTransfer>
    {
        private readonly List<Func<SignalUnlimintTransfer, ValueTask>> _list = new List<Func<SignalUnlimintTransfer, ValueTask>>();

        public SignalUnlimintTransferSubscriber(
            MyServiceBusTcpClient client,
            string queueName,
            TopicQueueType queryType)
        {
            client.Subscribe(SignalUnlimintTransfer.ServiceBusTopicName, queueName, queryType, Handler);
        }

        private async ValueTask Handler(IMyServiceBusMessage data)
        {
            var item = Deserializer(data.Data);

            if (!_list.Any())
            {
                throw new Exception("Cannot handle event. No subscribers");
            }

            foreach (var callback in _list)
            {
                await callback.Invoke(item);
            }
        }


        public void Subscribe(Func<SignalUnlimintTransfer, ValueTask> callback)
        {
            this._list.Add(callback);
        }

        private SignalUnlimintTransfer Deserializer(ReadOnlyMemory<byte> data) => data.ByteArrayToServiceBusContract<SignalUnlimintTransfer>();
    }
}
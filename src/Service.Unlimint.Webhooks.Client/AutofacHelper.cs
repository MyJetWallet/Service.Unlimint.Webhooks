using Autofac;
using Service.Unlimint.Webhooks.Grpc;

// ReSharper disable UnusedMember.Global

namespace Service.Unlimint.Webhooks.Client
{
    public static class AutofacHelper
    {
        public static void RegisterUnlimintWebhooksClient(this ContainerBuilder builder, string grpcServiceUrl)
        {
            var factory = new UnlimintWebhooksClientFactory(grpcServiceUrl);

            builder.RegisterInstance(factory.GetHelloService()).As<IHelloService>().SingleInstance();
        }
    }
}

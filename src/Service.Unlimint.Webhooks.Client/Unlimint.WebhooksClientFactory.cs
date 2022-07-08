using JetBrains.Annotations;
using MyJetWallet.Sdk.Grpc;
using Service.Unlimint.Webhooks.Grpc;

namespace Service.Unlimint.Webhooks.Client
{
    [UsedImplicitly]
    public class UnlimintWebhooksClientFactory: MyGrpcClientFactory
    {
        public UnlimintWebhooksClientFactory(string grpcServiceUrl) : base(grpcServiceUrl)
        {
        }

        public IHelloService GetHelloService() => CreateGrpcService<IHelloService>();
    }
}

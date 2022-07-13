using MyJetWallet.Sdk.Service;
using MyYamlParser;

namespace Service.Unlimint.Webhooks.Settings
{
    public class SettingsModel
    {
        [YamlProperty("UnlimintWebhooks.SeqServiceUrl")]
        public string SeqServiceUrl { get; set; }

        [YamlProperty("UnlimintWebhooks.ZipkinUrl")]
        public string ZipkinUrl { get; set; }

        [YamlProperty("UnlimintWebhooks.ElkLogs")]
        public LogElkSettings ElkLogs { get; set; }

        [YamlProperty("UnlimintWebhooks.SpotServiceBusHostPort")]
        public string SpotServiceBusHostPort { get; set; }

        [YamlProperty("UnlimintWebhooks.MyNoSqlReaderHostPort")]
        public string MyNoSqlReaderHostPort { get; set; }

        [YamlProperty("UnlimintWebhooks.BitgoDepositServiceGrpcUrl")]
        public string BitgoDepositServiceGrpcUrl { get; set; }

        [YamlProperty("UnlimintWebhooks.UnlimintSignerGrpcServiceUrl")]
        public string UnlimintSignerGrpcServiceUrl { get; set; }

        [YamlProperty("UnlimintWebhooks.WebHooksCheckerIntervalMSec")]
        public long WebHooksCheckerIntervalMSec { get; set; }

        [YamlProperty("UnlimintWebhooks.WebhooksUrl")]
        public string WebhooksUrl { get; set; }

        // [YamlProperty("UnlimintWebhooks.BlockchainWalletsGrpcServiceUrl")]
        // public string BlockchainWalletsGrpcServiceUrl { get; internal set; }
        //
        // [YamlProperty("UnlimintWebhooks.CircleWalletsGrpcServiceUrl")]
        // public string CircleWalletsGrpcServiceUrl { get; set; }
        //
        // [YamlProperty("UnlimintWebhooks.ClientWalletsGrpcServiceUrl")]
        // public string ClientWalletsGrpcServiceUrl { get; internal set; }
    }
}

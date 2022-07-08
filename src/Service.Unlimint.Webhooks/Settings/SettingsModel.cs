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
    }
}

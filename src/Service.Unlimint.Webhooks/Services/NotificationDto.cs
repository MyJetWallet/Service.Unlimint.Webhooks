using Newtonsoft.Json;

namespace Service.Unlimint.Webhooks.Services
{
    public class NotificationDto
    {
        [JsonProperty("callback_time")]
        public string CallbackTime { get; set; }

        [JsonProperty("card_account")]
        public string CardAccount { get; set; }

        [JsonProperty("cryptocurrency_account")]
        public string CryptocurrencyAccount { get; set; }

        [JsonProperty("customer")]
        public string Customer { get; set; }

        [JsonProperty("ewallet_account")]
        public string EwalletAccount { get; set; }

        [JsonProperty("merchant_order")]
        public string MerchantOrder { get; set; }

        [JsonProperty("payment_data")]
        public string PaymentData { get; set; }

        [JsonProperty("payment_method")]
        public string PaymentMethod { get; set; }
    }
}
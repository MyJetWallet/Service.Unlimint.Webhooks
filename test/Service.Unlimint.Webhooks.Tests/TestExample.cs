using System;
using MyJetWallet.Unlimint.Models.Payments;
using Newtonsoft.Json;
using NUnit.Framework;
using Service.Unlimint.Signer.Grpc.Models;
using Service.Unlimint.Webhooks.Services;

namespace Service.Unlimint.Webhooks.Tests
{
    public class TestExample
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            try
            {
                var jsonFull =
                    @"{""callback_time"":""2022-07-15T17:15:37.004Z"",""payment_method"":""BANKCARD"",""merchant_order"":{""id"":""3fdf58b6-2578-4b51-b02c-7e5054552409""},""customer"":{""email"":""yuriy.test.2022.07.15.001@mailinator.com"",""id"":""5e1c37e3230144a48ccb13b9662fc491"",""ip"":""20.82.12.154"",""locale"":""en""},""payment_data"":{""id"":""13278251"",""status"":""COMPLETED"",""amount"":1000.00,""currency"":""USD"",""created"":""2022-07-15T17:15:29.651783Z"",""note"":""jetwallet|-|5e1c37e3230144a48ccb13b9662fc491|-|SP-5e1c37e3230144a48ccb13b9662fc491"",""auth_code"":""vc1P0K"",""is_3d"":true},""card_account"":{""masked_pan"":""400000...0002"",""issuing_country_code"":""US"",""holder"":""TEST"",""token"":""12e5bcb8-705d-7ddf-dbbb-9a5fd696fb69"",""expiration"":""02/2025""}}";
                var jsonShort = @"{""merchant_order"":{""id"":""552409""}}";
                var callbackFull = JsonConvert.DeserializeObject<PaymentCallback>(jsonFull);
                var callbackShort = JsonConvert.DeserializeObject<PaymentCallback>(jsonShort);
                var info = new GetPaymentInfo
                {
                    Id = null,
                    Type = null,
                    MerchantOrderId = null,
                    MerchantWalletId = null,
                    Description = null,
                    Status = callbackFull?.PaymentData.Status,
                    Amount = null,
                    Fee = null,
                    Card = null,
                    TrackingRef = null,
                    ErrorCode = null,
                    Metadata = null,
                    CreateDate = null
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Assert.Fail();
            }
            //Console.WriteLine("Debug output");
            Assert.Pass("Pass");
        }
    }
}

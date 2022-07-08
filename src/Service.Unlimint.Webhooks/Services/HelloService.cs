using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Service.Unlimint.Webhooks.Grpc;
using Service.Unlimint.Webhooks.Grpc.Models;
using Service.Unlimint.Webhooks.Settings;

namespace Service.Unlimint.Webhooks.Services
{
    public class HelloService: IHelloService
    {
        private readonly ILogger<HelloService> _logger;

        public HelloService(ILogger<HelloService> logger)
        {
            _logger = logger;
        }

        public Task<HelloMessage> SayHelloAsync(HelloRequest request)
        {
            _logger.LogInformation("Hello from {name}", request.Name);

            return Task.FromResult(new HelloMessage
            {
                Message = "Hello " + request.Name
            });
        }
    }
}

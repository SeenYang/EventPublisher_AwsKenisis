using System;
using System.Threading;
using System.Threading.Tasks;
using EventPublisher.Clients;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EventPublisher.LocalTests
{
    internal static class Program
    {
        // public static AwsKinesisBusClient BusClient;
        private static IConfiguration _config;

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((_, services) =>
                {
                    var builder = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json", true, true);
                    _config = builder.Build();
                    services.UseEventBus(_config);
                });
        }

        private static async Task Main(string[] args)
        {
            using var host = CreateHostBuilder(args).Build();
            var client = host.Services.GetRequiredService<IEventBusClient>();

            // Send data into stream.
            while (!await client.GetEventBusStatus())
            {
                await Console.Error.WriteLineAsync("Event bus is not ready yet. Please wait.");
                Thread.Sleep(1000);
            }

            for (var j = 0; j < 10; ++j)
            {
                var message = "testData-" + j;
                await client.PublishEvent(message);
            }

            await Console.Error.WriteLineAsync("Done putting message into Kinesis.");
        }
    }
}
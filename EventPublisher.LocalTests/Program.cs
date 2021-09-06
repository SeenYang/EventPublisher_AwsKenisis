using System;
using System.Threading.Tasks;
using EventPublisher.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EventPublisher.LocalTests
{
    internal static class Program
    {
        // public static AwsKinesisBusClient BusClient;
        private static IConfigurationRoot _config;

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((_, services) =>
                {
                    var builder = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json", true, true);
                    _config = builder.Build();
                    services.UseEventBus(_config);
                    // TODO find a way to use base type.
                    services.Configure<AwsKinesisEventBusOptions>(_config.GetSection("EventBus"));
                });
        }

        private static async Task Main(string[] args)
        {
            using var host = CreateHostBuilder(args).Build();
            var client = host.Services.GetRequiredService<IEventBusClient>();

            // ** Please manually create stream before calling the initiate(). **
            await client.Initiate();

            // Send data into stream.
            for (var j = 0; j < 10; ++j)
            {
                var message = "testData-" + j;
                await client.PublishEvent(message);
            }

            await Console.Error.WriteLineAsync("Done putting message into Kinesis.");
        }
    }
}
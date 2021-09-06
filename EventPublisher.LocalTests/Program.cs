using System;
using System.Threading.Tasks;
using EventPublisher.Model;
using EventPublisher.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EventPublisher.LocalTests
{
    static class Program
    {
        // public static AwsKinesisBusClient BusClient;
        private static IConfigurationRoot _config;

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((_, services) =>
                {
                    var builder = new ConfigurationBuilder()
                        .AddJsonFile($"appsettings.json", true, true);
                    _config = builder.Build();
                    services.UseEventBus(_config);
                    // TODO find a way to use base type.
                    services.Configure<AwsKinesisEventBusOptions>(_config.GetSection("EventBus"));
                });
        }

        static async Task Main(string[] args)
        {
            using var host = CreateHostBuilder(args).Build();
            var client = host.Services.GetRequiredService<IEventBusClient>();

            // // Setup stream
            // var myStreamName = _config["EventBus:StreamName"];
            // await ((AwsKinesisBusClient)client).CreateStream(myStreamName, 1);


            await client.Initiate();

            // Send data into stream.
            for (var j = 0; j < 10; ++j)
            {
                var message = "testData-" + j;
                 await client.PublishEvent(message);
            }

            await Console.Error.WriteLineAsync($"Done putting message into Kinesis.");

            // Uncomment following if want to clean up stream after test.
            // client.ClosePublisher();


            // Setup Publisher
            // _busClient = new AwsKinesisBusClient();
            // var accessKey = config["AccessKey"] ?? "DUMMY_KEY";
            // var secretAccessKey = config["SecreteAccessKey"] ?? "DUMMY_KEY";
            // var serverUrl = config["ServerUrl"] ?? "http://localhost:4566";
            // var temp = config.GetSection("EventBus");
            //
            //
            // await _busClient.Initiate(new AwsKinesisEventBusOptions());
            //
            // // Setup stream
            // const string myStreamName = "myTestStream";
            // const int myStreamSize = 1;
            // _busClient.CreateStream(myStreamName, myStreamSize);
            //
            // // Check whether steam available
            // await WaitForStreamToBecomeAvailable(myStreamName);
            //
            // await Console.Error.WriteLineAsync("Putting records in stream : " + myStreamName);
            //
            // // Send data into stream.
            // for (var j = 0; j < 10; ++j)
            // {
            //     var partitionKey = "partitionKey-" + j;
            //     var message = "testData-" + j;
            //     var response = await _busClient.PublishEvent(message) as AwsKinesisPutRecordResponse;
            //     await Console.Error.WriteLineAsync(
            //         $"Successfully put record {j}:\n\t partition key = {partitionKey,15}, shard ID = {response.ShardId}");
            // }
            //
            // await Console.Error.WriteLineAsync($"Done putting message into Kinesis.");

            // Uncomment following if want to clean up stream after test.
            // _publisher.DeleteStream(myStreamName);
        }
    }
}
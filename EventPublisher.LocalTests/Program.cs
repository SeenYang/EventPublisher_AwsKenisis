using System;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Kinesis;

namespace EventPublisher.Producer
{
    static class Program
    {
        private static readonly AmazonKinesisClient KinesisClient = new AmazonKinesisClient(
            "DUMMY_KEY",
            "DUMMY_KEY",
            new AmazonKinesisConfig
            {
                ServiceURL = $"http://localhost:4566",
            });

        private static AwsKinesisPublisher _publisher;

        private static async Task Main(string[] args)
        {
            // Setup Publisher
            _publisher = new AwsKinesisPublisher();
            _publisher.Initiate("DUMMY_KEY", "DUMMY_KEY", "http://localhost:4566");

            // Setup stream
            const string myStreamName = "myTestStream";
            const int myStreamSize = 1;
            _publisher.CreateStream(myStreamName, myStreamSize);

            // Check whether steam available
            WaitForStreamToBecomeAvailable(myStreamName);
            
           await Console.Error.WriteLineAsync("Putting records in stream : " + myStreamName);
            
            // Send data into stream.
            for (var j = 0; j < 10; ++j)
            {
                var partitionKey = "partitionKey-" + j;
                var message = "testData-" + j;
                var response =  await _publisher.PutRecordAsync(myStreamName, partitionKey, message);
                await Console.Error.WriteLineAsync(
                    $"Successfully put record {j}:\n\t partition key = {partitionKey,15}, shard ID = {response.ShardId}");
            }

            await Console.Error.WriteLineAsync($"Done putting message into Kinesis.");
            
            // Uncomment following if want to clean up stream after test.
            // _publisher.DeleteStream(myStreamName);
        }

        /// <summary>
        /// This method waits a maximum of 10 minutes for the specified stream to become active.
        /// <param name="myStreamName">Name of the stream whose active status is waited upon.</param>
        /// </summary>
        private static void WaitForStreamToBecomeAvailable(string myStreamName)
        {
            var deadline = DateTime.UtcNow + TimeSpan.FromMinutes(10);
            while (DateTime.UtcNow < deadline)
            {
                var streamStatus = _publisher.GetStreamStatus(myStreamName);
                if (streamStatus == StreamStatus.ACTIVE)
                {
                    return;
                }
                Thread.Sleep(TimeSpan.FromSeconds(20));
            }

            throw new Exception("Stream " + myStreamName + " never went active.");
        }
    }
}
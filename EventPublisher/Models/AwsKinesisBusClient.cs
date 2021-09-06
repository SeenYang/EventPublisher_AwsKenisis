using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Amazon.Kinesis;
using Amazon.Kinesis.Model;
using Amazon.Runtime;
using EventPublisher.Model;
using EventPublisher.Models;
using DescribeStreamRequest = Amazon.Kinesis.Model.DescribeStreamRequest;
using PutRecordsRequest = Amazon.Kinesis.Model.PutRecordsRequest;

namespace EventPublisher
{
    public class AwsKinesisBusClient : IEventBusClient
    {
        private AmazonKinesisClient _client;
        
        public void Initiate()
        {
            throw new NotImplementedException();
        }

        public T PublishEvent<T>(IEventBase request)
        {
            throw new NotImplementedException();
        }

        public void ClosePublisher()
        {
            throw new NotImplementedException();
        }
        

        public void Initiate(string accessKeyId, string secretAccessKey, string serverUrl)
        {
            _client = new AmazonKinesisClient(
                accessKeyId,
                secretAccessKey,
                new AmazonKinesisConfig
                {
                    ServiceURL = serverUrl,
                });
        }

        public void CreateStream(string streamName, int streamSize)
        {
            // todo: verify whether client is initiated.
            try
            {
                var createStreamRequest = new CreateStreamRequest
                {
                    StreamName = streamName,
                    ShardCount = streamSize
                };
                var createStreamResponse = _client.CreateStreamAsync(createStreamRequest).Result;
                Console.Error.WriteLine($@"Created Stream :  {streamName} \  Response: {createStreamResponse}");
            }
            catch (ResourceInUseException)
            {
                Console.Error.WriteLine("Producer is quitting without creating stream " + streamName +
                                        " to put records into as a stream of the same name already exists.");
                Environment.Exit(1);
            }
        }

        public StreamStatus GetStreamStatus(string streamName)
        {
            var describeStreamReq = new DescribeStreamRequest
            {
                StreamName = streamName
            };
            var describeResult = _client.DescribeStreamAsync(describeStreamReq).Result;
            string streamStatus = describeResult.StreamDescription.StreamStatus;
            Console.Error.WriteLine("  - current state: " + streamStatus);

            return streamStatus;
        }

        public async Task<PutRecordResponse> PutRecordAsync<T>(string streamName, string partitionKey, T data)
        {
            var requestRecord = new PutRecordRequest
            {
                StreamName = streamName,
                Data = SerialiseData(data),
                PartitionKey = partitionKey
            };

            var result = await _client.PutRecordAsync(requestRecord);
            // todo: error handling.

            return result;
        }

        /// <summary>
        /// Error handling should be handled on consumer leve.
        /// TODO: PartitionKey need to find a way.
        /// </summary>
        /// <param name="streamName"></param>
        /// <param name="partitionKey"></param>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<PutRecordsResponse> PutRecordsAsync<T>(string streamName, string partitionKey,
            IEnumerable<T> data)
        {
            var enumerable = data as T[] ?? data.ToArray();
            if (data == null || !enumerable.Any())
            {
                throw new ArgumentNullException(nameof(data));
            }

            var recordsRequest = new PutRecordsRequest
            {
                Records = new List<PutRecordsRequestEntry>(),
                StreamName = streamName
            };

            foreach (var d in enumerable)
            {
                var entry = new PutRecordsRequestEntry
                {
                    Data = SerialiseData(d),
                    PartitionKey = partitionKey // todo: confirm this.
                };
                recordsRequest.Records.Add(entry);
            }

            return await _client.PutRecordsAsync(recordsRequest);
        }

        public void DeleteStream(string streamName)
        {
            // Uncomment the following if you wish to delete the stream here.
            Console.Error.WriteLine("Deleting stream : " + streamName);
            var deleteStreamReq = new DeleteStreamRequest
            {
                StreamName = streamName
            };

            try
            {
                _client.DeleteStreamAsync(deleteStreamReq).Wait();
                Console.Error.WriteLine("Stream is now being deleted : " + streamName);
            }
            catch (ResourceNotFoundException ex)
            {
                Console.Error.WriteLine("Stream could not be found; " + ex);
            }
            catch (AmazonClientException ex)
            {
                Console.Error.WriteLine("Error deleting stream; " + ex);
            }
        }

        // TODO：make encoding as an option either read from config or enum variable.
        // TODO: Memory usage and batch setup need to confirm.
        private static MemoryStream SerialiseData<T>(T data)
        {
            var dataAsJson = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                Converters = { new EventConverter() }
            });
            var dataAsBytes = Encoding.UTF8.GetBytes(dataAsJson);
            return new MemoryStream(dataAsBytes);
        }
    }

    public class EventConverter : JsonConverter<IEventBase>
    {
        // TODO: Implement this once package is used in message consuming.
        public override IEventBase Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(
            Utf8JsonWriter writer,
            IEventBase value,
            JsonSerializerOptions options)
        {
            switch (value)
            {
                case null:
                    throw new ArgumentNullException(nameof(value));
                default:
                {
                    Type type = value.GetType();
                    JsonSerializer.Serialize(writer, value, type, options);
                    break;
                }
            }
        }
    }

    // public interface IEvent
    // {
    //     string EventSchemaVersion { get; init; }
    // }
}
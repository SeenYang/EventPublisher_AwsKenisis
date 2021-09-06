using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.Kinesis;
using Amazon.Kinesis.Model;
using Amazon.Runtime;
using EventPublisher.Helpers;
using EventPublisher.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using DescribeStreamRequest = Amazon.Kinesis.Model.DescribeStreamRequest;
using PutRecordsRequest = Amazon.Kinesis.Model.PutRecordsRequest;

namespace EventPublisher.Models
{
    public class AwsKinesisBusClient : IEventBusClient
    {
        private AmazonKinesisClient _client;
        private string _streamName;

        private readonly IOptions<AwsKinesisEventBusOptions> _config;

        public AwsKinesisBusClient(IOptions<AwsKinesisEventBusOptions> options)
        {
            _config = options;
        }
        // private string _partitionKey;


        public async Task<bool> Initiate()
        {
            // if (_config is not AwsKinesisEventBusOptions kConfig)
            // {
            //     throw new ArgumentNullException(nameof(_config));
            // }
            
            Initiate(_config.Value.AccessKeyId, _config.Value.SecretAccessKey, _config.Value.ServerUrl);
            _streamName = _config.Value.StreamName;

            return await GetStreamStatus(_streamName) == StreamStatus.ACTIVE;
        }

        public async Task<bool> PublishEvent<T>(T data)
        {
            // TODO: put the event context inject here.
            // Thus consumer should only pass what the `T data` they want to sent into this method.

            var response = await PutRecordAsync<T>(new AwsKinesisPutRecordRequestDto<T>
            {
                Data = data,
                PartitionalKey = new Random().Next(5).ToString()    // random between 0 - 5.
            });
            return response;
        }

        public void ClosePublisher()
        {
            throw new NotImplementedException();
        }

        
        private void Initiate(string accessKeyId, string secretAccessKey, string serverUrl)
        {
            _client = new AmazonKinesisClient(
                accessKeyId,
                secretAccessKey,
                new AmazonKinesisConfig
                {
                    ServiceURL = serverUrl,
                });
        }

        public async Task CreateStream(string streamName, int streamSize)
        {
            // todo: verify whether client is initiated.
            try
            {
                var createStreamRequest = new CreateStreamRequest
                {
                    StreamName = streamName,
                    ShardCount = streamSize
                };
                var createStreamResponse = await _client.CreateStreamAsync(createStreamRequest);
                await Console.Error.WriteLineAsync($@"Created Stream :  {streamName} \  Response: {createStreamResponse}");
            }
            catch (ResourceInUseException)
            {
                await Console.Error.WriteLineAsync("Producer is quitting without creating stream " + streamName +
                                        " to put records into as a stream of the same name already exists.");
                Environment.Exit(1);
            }
        }

        public async Task<StreamStatus> GetStreamStatus(string streamName)
        {
            var describeStreamReq = new DescribeStreamRequest
            {
                StreamName = streamName
            };
            var describeResult = await _client.DescribeStreamAsync(describeStreamReq);
            string streamStatus = describeResult.StreamDescription.StreamStatus;
            await Console.Error.WriteLineAsync("  - current state: " + streamStatus);

            return streamStatus;
        }

        private async Task<bool> PutRecordAsync<T>(AwsKinesisPutRecordRequestDto<T> request)
        {
            var requestRecord = new PutRecordRequest
            {
                StreamName = _streamName,
                Data = SerialiseData(request.Data, Encoding.UTF8),
                PartitionKey = request.PartitionalKey
            };

            var result = await _client.PutRecordAsync(requestRecord);
            await Console.Error.WriteLineAsync(
                $"Successfully put record {request.Data.ToString()}:\n\t shard ID = {result.ShardId}");
            
            // todo: error handling.

            return true ;
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
                    Data = SerialiseData(d, Encoding.UTF8),
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
        private static MemoryStream SerialiseData<T>(T data, Encoding encoding)
        {
            var dataAsJson = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                Converters = { new EventConverter() }
            });
            var dataAsBytes = encoding.GetBytes(dataAsJson);
            return new MemoryStream(dataAsBytes);
        }
    }
}
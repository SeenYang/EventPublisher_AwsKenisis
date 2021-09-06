using System;

namespace EventPublisher.Models
{
    public interface IEventBusOptionBase
    {
        public const EventBusTypeEnum Type = EventBusTypeEnum.Default;
    }

    // public class AwsKinesisEventBusOptions : IEventBusOptionBase
    public class AwsKinesisEventBusOptions
    {
        public string AccessKeyId { get; set; }
        public string SecretAccessKey { get; set; }
        public string ServerUrl { get; set; }
        public string StreamName { get; set; }
        public const EventBusTypeEnum Type  = EventBusTypeEnum.Kinesis;
    }
}
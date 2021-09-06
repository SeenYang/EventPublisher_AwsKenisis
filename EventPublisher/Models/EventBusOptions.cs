using System;

namespace EventPublisher.Models
{
    public interface IEventBusOptionBase
    {
        public EventBusTypeEnum Type { get; set; }
    }

    public class AwsKinesisEventBusOptions : IEventBusOptionBase
    {
        public string AccessKeyId { get; set; }
        public string SecretAccessKey { get; set; }
        public string ServerUrl { get; set; }
        public string StreamName { get; set; }
        public EventBusTypeEnum Type { get; set; } = EventBusTypeEnum.Kinesis;
    }
}
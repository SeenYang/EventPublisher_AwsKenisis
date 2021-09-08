using EventPublisher.Model;

namespace EventPublisher.Models
{
    public class PaymentsEventBusOptions : IEventBusOptionBase
    {
        public const EventBusTypeEnum Type = EventBusTypeEnum.Payments;
        public string AccessKeyId { get; set; }
        public string SecretAccessKey { get; set; }
        public string ServerUrl { get; set; }
        public string StreamName { get; set; }
    }
}
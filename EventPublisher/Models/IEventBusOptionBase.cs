using EventPublisher.Model;

namespace EventPublisher.Models
{
    public interface IEventBusOptionBase
    {
        public const EventBusTypeEnum Type = EventBusTypeEnum.Default;
    }
}

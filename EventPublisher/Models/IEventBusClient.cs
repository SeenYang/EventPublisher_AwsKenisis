using EventPublisher.Model;

namespace EventPublisher.Models
{
    public interface IEventPublisher
    {
        void Initiate();
        T PublishEvent<T>(IEventBase request);
        void ClosePublisher();
    }
}
using System.Threading.Tasks;

namespace EventPublisher.Clients
{
    public interface IEventBusClient
    {
        Task<bool> PublishEvent<T>(T data);
        Task<bool> GetEventBusStatus();
    }
}
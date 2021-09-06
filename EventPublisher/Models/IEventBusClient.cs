using System.Threading.Tasks;
using EventPublisher.Model;
using Microsoft.Extensions.Configuration;

namespace EventPublisher.Models
{
    public interface IEventBusClient
    {
        Task<bool> Initiate();
        Task<bool> PublishEvent<T>(T data);
        void ClosePublisher();
    }
}
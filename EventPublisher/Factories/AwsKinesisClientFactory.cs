using EventPublisher.Models;

namespace EventPublisher.Factories
{
    public class AwsKinesisClientFactory : IEventBusClientFactory
    {
        private IEventBusClient _client;

        // public IEventBusClient GetEventBusClient()
        // {
        //     _client = new AwsKinesisBusClient();
        //
        //     return _client;
        // }
    }
}
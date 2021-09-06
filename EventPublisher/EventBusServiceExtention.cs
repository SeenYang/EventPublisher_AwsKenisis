using System;
using EventPublisher.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventPublisher
{
    public static class EventBusServiceExtensions
    {
        public static IServiceCollection UseEventBus(this IServiceCollection services, IConfiguration config)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            var type = config["EventBus:Type"];
            if (string.IsNullOrWhiteSpace(type))
            {
                throw new ArgumentNullException(nameof(config), "EventBus Type is not provided.");
            }
            Enum.TryParse<EventBusTypeEnum>(type, out var parsedType);
            
            switch (parsedType)
            {
                case EventBusTypeEnum.Kinesis:
                    services.Configure<AwsKinesisEventBusOptions>(config.GetSection("EventBus"));
                    services.AddSingleton<IEventBusClient, AwsKinesisBusClient>();
                    break;
                case EventBusTypeEnum.Default:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentNullException();
            }
            
            return services;
        }
    }
}
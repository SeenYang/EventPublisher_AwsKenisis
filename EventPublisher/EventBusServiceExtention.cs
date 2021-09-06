using Microsoft.Extensions.DependencyInjection;
using System;
using EventPublisher.Models;
using Microsoft.Extensions.Configuration;

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
            return RegisterClient(services, parsedType);
        }

        private static IServiceCollection RegisterClient(this IServiceCollection services, EventBusTypeEnum type)
        {
            switch (type)
            {
                case EventBusTypeEnum.Kinesis:
                    services.AddSingleton<IEventBusClient, AwsKinesisBusClient>();
                    break;
                default:
                    throw new ArgumentNullException();
            }
            return services;
        }
    }
}
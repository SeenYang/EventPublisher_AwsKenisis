using System;
using EventPublisher.Clients;
using EventPublisher.Model;
using EventPublisher.Models;
using EventPublisher.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventPublisher
{
    public static class EventBusServiceExtensions
    {
        public static IServiceCollection UseEventBus(this IServiceCollection services, IConfiguration config)
        {
            if (services is null)
                throw new ArgumentNullException(nameof(services));
            if (config is null)
                throw new ArgumentNullException(nameof(config));

            var type = config["EventBus:Type"];
            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentNullException(nameof(config), "EventBus Type is not provided.");
            Enum.TryParse<EventBusTypeEnum>(type, out var parsedType);

            switch (parsedType)
            {
                case EventBusTypeEnum.Payments:
                    services.Configure<PaymentsEventBusOptions>(config);
                    services.AddSingleton<IEventBusClient, AwsKinesisBusClient>();
                    break;
                case EventBusTypeEnum.Default:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentNullException();
            }

            return services;
        }

        /// <summary>
        ///     TODO: This is WIP feature that haven't implemented.
        ///     Please use above one by passing IServiceCollection and IConfiguration.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="optionsAction"></param>
        /// <param name="contextLifeTime"></param>
        /// <param name="optionsLifetime"></param>
        /// <typeparam name="TEventBusOptions"></typeparam>
        /// <returns></returns>
        public static IServiceCollection UseEventBus<TEventBusOptions>(
            this IServiceCollection services,
            Action<PaymentsEventBusOptions> optionsAction = null,
            ServiceLifetime contextLifeTime = ServiceLifetime.Scoped,
            ServiceLifetime optionsLifetime = ServiceLifetime.Scoped
        ) where TEventBusOptions : IEventBusOptionBase
        {
            return services;
        }
    }
}
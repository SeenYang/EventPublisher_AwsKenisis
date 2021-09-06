using System;
using System.Collections.Generic;
using EventPublisher.Factories;
using EventPublisher.Models;

namespace EventPublisher.Strategies
{
    public class EventBusCreateStrategy
    {
        // TODO: Make input as IOptions
        public Type GetEventBusType(EventBusTypeEnum type)
        {
            switch (type)
            {
                case EventBusTypeEnum.Kinesis:
                    return typeof(AwsKinesisClientFactory);
                default:
                    throw new ArgumentNullException();
            }
        }
    }
}
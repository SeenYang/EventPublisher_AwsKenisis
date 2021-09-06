using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using EventPublisher.Model;

namespace EventPublisher.Helpers
{
    public class EventConverter : JsonConverter<EventBase>
    {
        // TODO: Implement this once package is used in message consuming.
        public override EventBase Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(
            Utf8JsonWriter writer,
            EventBase value,
            JsonSerializerOptions options)
        {
            switch (value)
            {
                case null:
                    throw new ArgumentNullException(nameof(value));
                default:
                {
                    Type type = value.GetType();
                    JsonSerializer.Serialize(writer, value, type, options);
                    break;
                }
            }
        }
    }
}
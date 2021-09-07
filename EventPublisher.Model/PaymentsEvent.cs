namespace EventPublisher.Model
{
    public class PaymentsEvent<T> : EventBase
    {
        private string EventSchemaVersion { get; set; } = "1.0";
        private string DataContentType { get; set; } = "application/json";
        // Here is the type that actual sent to event bus. we should hide this from consumer.
        private string Type { get; set; } = "EventBusType";
        public string PartitionalKey { get; set; }
        public T Data { get; set; }
    }
}
namespace EventPublisher.Model
{
    public class PaymentsEvent<T> : EventBase
    {
        private string EventSchemaVersion { get; set; } = "1.0";
        private string DataContentType { get; set; } = "application/json";
        private string Type { get; set; } = "xero.payments.statusupdated.v1.1";
        public string PartitionalKey { get; set; }
        public T Data { get; set; }
    }
}
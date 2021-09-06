namespace EventPublisher.Model
{
    public class AwsKinesisPutRecordRequestDto<T> : EventBase
    {
        public string PartitionalKey { get; set; }
        public T Data { get; set; }
    }
}
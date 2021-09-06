namespace EventPublisher.Model
{
    public class KinesisConfig : IClientConfig
    {
        public string StreamName { get; set; }
        public string PartitionKey { get; set; }
    }
}
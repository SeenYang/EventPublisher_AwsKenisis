namespace EventPublisher.Model
{
    public class AwsKinesisConfig
    {
        public string StreamName { get; set; }
        public int StreamSize { get; set; } = 1;
        public string AccessKeyId { get; set; }
        public string SecretAccessKey { get; set; }
        public string ServerUrl { get; set; }
    }
}
namespace EventPublisher.Model
{
    public  interface IEventBase
    {
        string EventSchemaVersion { get; set; }
    }
}
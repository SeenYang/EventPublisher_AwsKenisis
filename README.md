#EventPublisher


## Usage
Following is sampel usage of this lib.
```csharp
public class Startup
{
	private IConfiguration Configuration { get; }
	
	public void ConfigureServices(IServiceCollection services)
	{
		services.UseEventBus(Configuration);
	}
}

public ConsumerClass
{
	private readonly IEventBusClient _client;

	public ConsumerClass(IEventBusClient client){
		_client = client;
	}

	public void Usage(){
		_client.Initiate();
		_client.PublishEvent(new CusomeriseEvent());
	}
}

```

appsettings.json
```json
{
  "EventBus": {
    "Type": "Kinesis",
    "AccessKeyId": "DUMMY_KEY",
    "SecretAccessKey": "DUMMY_SECRET_KEY",
    "ServerUrl": "http://localhost:4566",
    "StreamName": "myTestStream"
  }
}
```

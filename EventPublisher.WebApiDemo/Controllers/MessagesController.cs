using System;
using System.Threading;
using System.Threading.Tasks;
using EventPublisher.Clients;
using Microsoft.AspNetCore.Mvc;

namespace EventPublisher.WebApiDemo.Controllers
{
    [Route("api/Message")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IEventBusClient _client;

        public MessagesController(IEventBusClient client)
        {
            _client = client;
        }

        [HttpPost]
        public async Task<IActionResult> PublishMessage(string message)
        {
            while (!await _client.GetEventBusStatus())
            {
                await Console.Error.WriteLineAsync("Event bus is not ready yet. Please wait.");
                Thread.Sleep(1000);
            }

            await _client.PublishEvent(message);
            return Ok();
        }
    }
}
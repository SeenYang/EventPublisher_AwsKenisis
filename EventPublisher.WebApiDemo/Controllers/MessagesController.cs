using System.Threading.Tasks;
using EventPublisher.Models;
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
            await _client.Initiate();
            await _client.PublishEvent(message);
            return Ok();
        }
    }
}
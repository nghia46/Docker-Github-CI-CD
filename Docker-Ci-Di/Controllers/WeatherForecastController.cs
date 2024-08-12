using Docker_Ci_Di.AMQP;
using Microsoft.AspNetCore.Mvc;

namespace Docker_Ci_Di.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController(IMessagePublisher messagePublisher) : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Rét buốt", "Lạnh buốt", "Lạnh lẽo", "Mát mẻ", "Ôn hòa", "Ấm áp", "Dịu nhẹ", "Nóng", "Oi bức",
            "Nóng như thiêu đốt"
        };
        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-1000, 1000),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
        [HttpGet("Greetings")]
        public IActionResult GetGreetings()
        {
            return Ok("Hello from k8s! v2");
        }

        [HttpGet("Publish-Message")]
        public async Task<IActionResult> PublishMessage(string mgs)
        {
            try
            {
                var message = new TextMessage
                {
                    Text = mgs
                };
                await messagePublisher.PublishAsync(message, QueueName.TestQueue, CancellationToken.None);
            }
            catch (Exception e)
            {
                return BadRequest(e.StackTrace);
                throw;
            }

            return Ok($"Message is publish to {QueueName.TestQueue}");
        }
    }
}

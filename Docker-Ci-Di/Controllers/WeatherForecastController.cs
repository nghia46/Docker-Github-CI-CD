using Microsoft.AspNetCore.Mvc;

namespace Docker_Ci_Di.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Rét buốt", "Lạnh buốt", "Lạnh lẽo", "Mát mẻ", "Ôn hòa", "Ấm áp", "Dịu nhẹ", "Nóng", "Oi bức", "Nóng như thiêu đốt"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

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
    }
}

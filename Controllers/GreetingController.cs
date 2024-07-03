using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace HNGTask2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GreetingController : ControllerBase
    {
        private readonly ILogger<GreetingController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public GreetingController(ILogger<GreetingController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("Greeting")]
        public async Task<IActionResult> GetGreeting([FromQuery] string visitor_name)
        {
            try
            {
                string clientIp = HttpContext.Connection.RemoteIpAddress.ToString();
                _logger.LogInformation($"Initial client IP: {clientIp}");

                if (HttpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
                {
                    clientIp = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()?.Split(',').FirstOrDefault()?.Trim();
                    _logger.LogInformation($"Client IP from X-Forwarded-For: {clientIp}");
                }

                if (IPAddress.TryParse(clientIp, out IPAddress ip))
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                    {
                        clientIp = ip.MapToIPv4().ToString();
                        _logger.LogInformation($"Converted IPv6 to IPv4: {clientIp}");
                    }
                }

                var location = await GetLocationFromIp(clientIp);
                var temperature = await GetTemperatureFromLocation(location);

                var greeting = $"Hello, {visitor_name}!, the temperature is {temperature} degrees Celsius in {location}";

                var response = new
                {
                    client_ip = clientIp,
                    location,
                    greeting
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the greeting request.");
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        private async Task<string> GetLocationFromIp(string ip)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var response = await httpClient.GetAsync($"https://ipinfo.io/{ip}/json?token=dd7de95ab4936f");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();

                var jsonDoc = JsonDocument.Parse(content);

                if (jsonDoc.RootElement.TryGetProperty("city", out var cityElement))
                {
                    return cityElement.GetString();
                }
                else
                {
                    _logger.LogWarning("City information not found in IP geolocation response.");
                    return "Unknown";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the location from the IP address.");
                throw;
            }
        }

        private async Task<int> GetTemperatureFromLocation(string location)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var apiKey = "3d96a0abab4746ccbbd91853240207";
                var response = await httpClient.GetAsync($"https://api.weatherapi.com/v1/current.json?key={apiKey}&q={location}");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var jsonDoc = JsonDocument.Parse(content);
                var temperature = jsonDoc.RootElement.GetProperty("current").GetProperty("temp_c").GetInt32();
                return temperature;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the temperature from the location.");
                throw;
            }
        }
    }
}

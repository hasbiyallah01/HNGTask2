/*using Microsoft.AspNetCore.Http;
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

        private async Task<string> GetLocationFromIp(string ip)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync($"https://ipinfo.io/{ip}/json?token=dd7de95ab4936f");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            // Parse JSON content
            var jsonDoc = JsonDocument.Parse(content);

            // Check if the 'city' property exists
            if (jsonDoc.RootElement.TryGetProperty("city", out var cityElement))
            {
                var city = cityElement.GetString();
                return city;
            }
            else
            {
                // Handle case where 'city' property is missing
                _logger.LogWarning("City information not found in IP geolocation response.");
                return "Unknown"; // Or handle accordingly based on your application's logic
            }
        }


        private async Task<int> GetTemperatureFromLocation(string location)
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

        *//*private async Task<string> GetLocationFromIp(string ip)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var apiKey = "510790b9a55b44c5ab0d851c1ae626f5";
            var response = await httpClient.GetAsync($"https://api.ipgeolocation.io/ipgeo?apiKey={apiKey}&ip={ip}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(content);
            var city = jsonDoc.RootElement.GetProperty("city").GetString();
            return city;
        }*/

/*private async Task<int> GetTemperatureFromLocation(string location)
{
    var httpClient = _httpClientFactory.CreateClient();
    var response = await httpClient.GetAsync($"https://api.weatherapi.com/v1/current.json?key=YOUR_API_KEY&q={location}");
    response.EnsureSuccessStatusCode();
    var content = await response.Content.ReadAsStringAsync();
    var jsonDoc = JsonDocument.Parse(content);
    var temperature = jsonDoc.RootElement.GetProperty("current").GetProperty("temp_c").GetInt32();
    return temperature;
}*//*


}
}


*/



using Microsoft.AspNetCore.Http;
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

        private async Task<string> GetLocationFromIp(string ip)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync($"https://ipinfo.io/{ip}/json?token=dd7de95ab4936f");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            // Parse JSON content
            var jsonDoc = JsonDocument.Parse(content);

            // Check if the 'city' property exists
            if (jsonDoc.RootElement.TryGetProperty("city", out var cityElement))
            {
                var city = cityElement.GetString();
                return city;
            }
            else
            {
                // Handle case where 'city' property is missing
                _logger.LogWarning("City information not found in IP geolocation response.");
                return "Unknown"; // Or handle accordingly based on your application's logic
            }
        }

        private async Task<int> GetTemperatureFromLocation(string location)
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
    }
}

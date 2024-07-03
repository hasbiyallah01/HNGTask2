using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace HNGTask2.Controllers
{
    [ApiController]
    [Route("api/myip")]
    public class GreetingController : ControllerBase
    {
        private readonly IpApiClient _ipApiClient;

        public GreetingController(IpApiClient ipApiClient)
        {
            _ipApiClient = ipApiClient;
        }

        [HttpGet]
        public async Task<ActionResult> Get(CancellationToken ct)
        {
            try
            {
                var ipAddress = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? HttpContext.Connection.RemoteIpAddress?.ToString();
                var ipAddressWithoutPort = ipAddress?.Split(':')[0];

                var ipApiResponse = await _ipApiClient.Get(ipAddressWithoutPort, ct);

                var response = new
                {
                    IpAddress = ipAddressWithoutPort,
                    Country = ipApiResponse?.country,
                    Region = ipApiResponse?.regionName,
                    City = ipApiResponse?.city,
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }

    public class IpApiClient
    {
        private const string BASE_URL = "http://ip-api.com";
        private readonly HttpClient _httpClient;

        public IpApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IpApiResponse?> Get(string? ipAddress, CancellationToken ct)
        {
            var route = $"{BASE_URL}/json/{ipAddress}";
            var response = await _httpClient.GetFromJsonAsync<IpApiResponse>(route, ct);
            return response;
        }
    }

    public class IpApiResponse
    {
        public string? country { get; set; }
        public string? regionName { get; set; }
        public string? city { get; set; }
    }

    public class OpenWeatherMapClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public OpenWeatherMapClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["OpenWeatherMap:ApiKey"];
        }

        public async Task<WeatherResponse> GetWeatherAsync(string city, CancellationToken ct)
        {
            var response = await _httpClient.GetAsync(
                $"http://api.openweathermap.org/data/2.5/weather?q={city}&appid={_apiKey}&units=metric",
                ct);

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync(ct);
            return JsonSerializer.Deserialize<WeatherResponse>(content);
        }
    }

    public class WeatherResponse
    {
        public MainInfo Main { get; set; }
    }

    public class MainInfo
    {
        public float Temp { get; set; }
    }
}

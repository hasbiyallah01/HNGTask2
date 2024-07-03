using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace HNGTask2.Controllers
{
    [ApiController]
    [Route("api/myip")]
    public class GreetingController(IpApiClient ipApiClient) : ControllerBase
    {
        private readonly IpApiClient _ipApiClient = ipApiClient;
    
        [HttpGet]
        public async Task<ActionResult> Get(CancellationToken ct)
        {
            try
            {
                var ipAddress = HttpContext.GetServerVariable("HTTP_X_FORWARDED_FOR") ?? HttpContext.Connection.RemoteIpAddress?.ToString();
                var ipAddressWithoutPort = ipAddress?.Split(':')[0];
    
                var ipApiResponse = await _ipApiClient.Get(ipAddressWithoutPort, ct);

                var response = new
                {
                    IpAddress = ipAddressWithoutPort,
                    Country = ipApiResponse?.country,
                    Region = ipApiResponse?.regionName,
                    City = ipApiResponse?.city,
                    //District = ipApiResponse?.district,
                    //PostCode = ipApiResponse?.zip,
                    //Longitude = ipApiResponse?.lon.GetValueOrDefault(),
                    //Latitude = ipApiResponse?.lat.GetValueOrDefault(),
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
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

    public class IpApiClient(HttpClient httpClient)
    {
        private const string BASE_URL = "http://ip-api.com";
        private readonly HttpClient _httpClient = httpClient;
    
        public async Task<IpApiResponse?> Get(string? ipAddress, CancellationToken ct)
        {
            var route = $"{BASE_URL}/json/{ipAddress}";
            var response = await _httpClient.GetFromJsonAsync<IpApiResponse>(route, ct);
            return response;
        }
    }

    public sealed class IpApiResponse
    {
        public string? name { get; set; }
        public string? status { get; set; }
        public double? temperature { get; set; }
        public string? continent { get; set; }
        public string? country { get; set; }
        public string? regionName { get; set; }
        public string? city { get; set; }
        //public string? district { get; set; }
        //public string? zip { get; set; }
        //public double? lat { get; set; }
        //public double? lon { get; set; }
        //public string? isp { get; set; }
        public string? query { get; set; }
    }


}

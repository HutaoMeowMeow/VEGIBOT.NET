using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VEGIBOT.Models;

namespace VEGIBOT.Controllers
{
    [Authorize]
    public class WeatherController : Controller
    {
        private readonly IConfiguration _cfg;

        public WeatherController(IConfiguration cfg)
        {
            _cfg = cfg;
        }

        // GET /Weather
        public IActionResult Index()
        {
            var apiKey = _cfg["Weather:ApiKey"];

            if (string.IsNullOrEmpty(apiKey))
            {
                var vm = new WeatherViewModel
                {
                    IsConfigured = false,
                    SetupMessage = "Weather API key not configured. Add a valid OpenWeatherMap API key to appsettings.json to enable weather data."
                };
                return View(vm);
            }

            // If key is configured, we would make a real API call here
            // For now, show setup message since we don't have a real key in this demo
            var vm2 = new WeatherViewModel
            {
                IsConfigured = true,
                SetupMessage = "Weather API key is configured but real API integration would be performed here."
            };
            return View(vm2);
        }
    }
}
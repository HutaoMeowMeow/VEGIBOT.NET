using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VEGIBOT.Models;

namespace VEGIBOT.Controllers
{
    [Authorize]
    public class GardenAIController : Controller
    {
        private readonly IConfiguration _cfg;

        public GardenAIController(IConfiguration cfg)
        {
            _cfg = cfg;
        }

        // GET /GardenAI
        public IActionResult Index()
        {
            var vm = new AiChatViewModel
            {
                IsConfigured = false,
                ConfigurationNote = "Garden AI API key not configured. Add an API key to appsettings.json to enable the Garden AI assistant."
            };
            return View(vm);
        }

        // POST /GardenAI/Chat - send a message to the AI
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Chat(AiChatMessage message)
        {
            if (message == null) return View("Index");

            // In a full implementation, this would call an AI API
            // For now, return a helpful response based on context
            var response = new AiChatMessage
            {
                Role = "assistant",
                Content = "Garden AI is not currently configured. Please add an API key to appsettings.json to enable personalized gardening assistance.",
                Timestamp = DateTime.Now
            };

            var vm = new AiChatViewModel
            {
                IsConfigured = false,
                ConfigurationNote = "Garden AI API key not configured.",
                Messages = new List<AiChatMessage> { message, response }
            };
            return View("Index", vm);
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VEGIBOT.Models;
using VEGIBOT.Services;

namespace VEGIBOT.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        // GET: / or /Home/Index  →  Dashboard
        public async Task<IActionResult> Index()
        {
            var user     = await _userManager.GetUserAsync(User);
            var userName = user?.Email ?? User.Identity?.Name ?? "Gardener";
            // Show just the local-part of the email as a friendly name
            var displayName = userName.Contains('@')
                ? userName[..userName.IndexOf('@')]
                : userName;

            var vm = DashboardDemoService.BuildViewModel(displayName);
            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VEGIBOT.Data;
using VEGIBOT.Models;

namespace VEGIBOT.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public SettingsController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        private string UserId() => _userManager.GetUserId(User)!;

        // GET /Settings
        public async Task<IActionResult> Index()
        {
            var userId = UserId();
            var saved = await _db.UserSettings.AsNoTracking().FirstOrDefaultAsync(s => s.UserId == userId);

            var vm = saved == null
                ? new SettingsViewModel { DisplayName = User.Identity?.Name }
                : new SettingsViewModel
                {
                    DisplayName = saved.DisplayName ?? User.Identity?.Name,
                    TemperatureUnit = saved.TemperatureUnit,
                    DateFormat = saved.DateFormat,
                    NotificationsEnabled = saved.NotificationsEnabled,
                    EmailReminders = saved.EmailReminders,
                    WeeklyDigest = saved.WeeklyDigest,
                    GardenClimate = saved.GardenClimate,
                    GardenLocation = saved.GardenLocation
                };

            return View(vm);
        }

        // POST /Settings/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(SettingsViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", vm);
            }

            var userId = UserId();
            var settings = await _db.UserSettings.FirstOrDefaultAsync(s => s.UserId == userId);

            if (settings == null)
            {
                settings = new UserSettings { UserId = userId };
                _db.UserSettings.Add(settings);
            }

            settings.DisplayName = vm.DisplayName;
            settings.TemperatureUnit = vm.TemperatureUnit;
            settings.DateFormat = vm.DateFormat;
            settings.NotificationsEnabled = vm.NotificationsEnabled;
            settings.EmailReminders = vm.EmailReminders;
            settings.WeeklyDigest = vm.WeeklyDigest;
            settings.GardenClimate = vm.GardenClimate;
            settings.GardenLocation = vm.GardenLocation;
            settings.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            TempData["Success"] = "Settings saved.";
            return RedirectToAction(nameof(Index));
        }
    }
}

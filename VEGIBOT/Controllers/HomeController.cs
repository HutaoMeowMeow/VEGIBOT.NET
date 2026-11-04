using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace VEGIBOT.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly global::VEGIBOT.Data.ApplicationDbContext _db;

        public HomeController(UserManager<IdentityUser> userManager, global::VEGIBOT.Data.ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        // GET: / or /Home/Index  →  Dashboard
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var userName = user?.UserName ?? user?.Email ?? User.Identity?.Name ?? "Gardener";
            // Show just the local-part of the email as a friendly name
            var displayName = userName.Contains('@')
                ? userName[..userName.IndexOf('@')]
                : userName;

            // Get statistics from database
            var totalPlants = _db.Plants.Count(p => p.UserId == user!.Id);
            var healthyPlants = _db.Plants.Count(p => p.UserId == user!.Id && (p.Health == global::VEGIBOT.Models.HealthStatus.Excellent || p.Health == global::VEGIBOT.Models.HealthStatus.Good));
            var pendingTasks = _db.PlantTasks.Count(t => t.UserId == user!.Id && !t.IsCompleted);
            var upcomingReminders = _db.Reminders.Count(r => r.UserId == user!.Id && r.IsEnabled && !r.IsCompleted && r.ReminderDate >= DateTime.Today && r.ReminderDate <= DateTime.Today.AddDays(7));

            // Get plants from database
            var plants = GetPlantsForUser(user!.Id);

            // Get tasks from database
            var tasks = GetPendingTasks(user!.Id);

            // Weather - show setup message if not configured
            var weather = new global::VEGIBOT.Models.WeatherDemo
            {
                Condition = "Weather API not configured",
                ConditionIcon = "🌤️",
                TemperatureCelsius = 0,
                HumidityPercent = 0,
                WindKph = 0,
                GardeningTip = "Configure a weather API key in appsettings.json to enable weather data.",
                IsDemoData = false
            };

            // Recent activity from database
            var historyEntries = _db.PlantHistory
                .Where(h => h.UserId == user!.Id)
                .OrderByDescending(h => h.OccurredAt)
                .Take(5)
                .ToList();

            var recentActivity = MapHistoryToActivityItems(historyEntries);

            // Insights
            var insights = new List<global::VEGIBOT.Models.GardenInsight>
            {
                new global::VEGIBOT.Models.GardenInsight
                {
                    Title = "Consistent Watering",
                    Body = "Most vegetables prefer deep, infrequent watering over shallow daily irrigation. Aim for 2–3 times per week depending on weather.",
                    IconEmoji = "💧",
                    TagLabel = "Watering"
                },
                new global::VEGIBOT.Models.GardenInsight
                {
                    Title = "Tomato Care",
                    Body = "Remove side shoots (suckers) from indeterminate tomato varieties regularly to focus energy into fruit development.",
                    IconEmoji = "🍅",
                    TagLabel = "Pruning"
                }
            };

            // Build the view model
            var vm = new global::VEGIBOT.Models.DashboardViewModel
            {
                UserName = displayName,
                Greeting = GetGreeting(),
                Now = DateTime.Now,
                IsDemoData = false,

                Stats = new List<global::VEGIBOT.Models.StatCard>
                {
                    new global::VEGIBOT.Models.StatCard
                    {
                        Label = "Total Plants",
                        Value = totalPlants.ToString(),
                        ColorClass = "stat-green",
                        IconSvgPath = "M12 2C8 6 5 10 5 14a7 7 0 0 0 14 0c0-4-3-8-7-12z M12 14v6 M12 12l3-3 M12 14l-3-3"
                    },
                    new global::VEGIBOT.Models.StatCard
                    {
                        Label = "Healthy Plants",
                        Value = healthyPlants.ToString(),
                        ColorClass = "stat-teal",
                        IconSvgPath = "M22 11.08V12a10 10 0 1 1-5.93-9.14 M22 4 12 14.01l-3-3"
                    },
                    new global::VEGIBOT.Models.StatCard
                    {
                        Label = "Pending Tasks",
                        Value = pendingTasks.ToString(),
                        ColorClass = "stat-amber",
                        IconSvgPath = "M9 11l3 3L22 4 M21 12v7a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h11"
                    },
                    new global::VEGIBOT.Models.StatCard
                    {
                        Label = "Upcoming Reminders",
                        Value = upcomingReminders.ToString(),
                        ColorClass = "stat-rose",
                        IconSvgPath = "M18 8A6 6 0 0 0 6 8c0 7-3 9-3 9h18s-3-2-3-9 M13.73 21a2 2 0 0 1-3.46 0"
                    }
                },

                Plants = plants,

                Tasks = tasks,

                Weather = weather,

                RecentActivity = recentActivity,

                Insights = insights
            };

            return View(vm);
        }

        // Helper method to get plants for a user (avoids expression tree issues)
        private List<global::VEGIBOT.Models.PlantCard> GetPlantsForUser(string userId)
        {
            return _db.Plants
                .Where(p => p.UserId == userId)
                .OrderBy(p => p.Name)
                .Select(p => new global::VEGIBOT.Models.PlantCard
                {
                    Id = p.Id,
                    Name = p.Name,
                    Variety = p.Variety,
                    IconEmoji = p.IconEmoji,
                    ColorAccent = p.ColorAccent,
                    PlantedDate = p.PlantedDate,
                    Stage = p.Stage,
                    Health = p.Health,
                    GrowthPercent = 70, // Default growth progress
                    Notes = p.Notes
                }).ToList();
        }

        // Helper method to get pending tasks for a user (avoids expression tree issues)
        private List<global::VEGIBOT.Models.CareTask> GetPendingTasks(string userId)
        {
            var result = new List<global::VEGIBOT.Models.CareTask>();
            var query = _db.PlantTasks
                .Include(t => t.Plant)
                .Where(t => t.UserId == userId && !t.IsCompleted)
                .OrderBy(t => t.DueDate)
                .Take(5);
            foreach (var t in query)
            {
                var plantName = t.Plant?.Name ?? "General";
                result.Add(new global::VEGIBOT.Models.CareTask
                {
                    Id = t.Id,
                    Title = t.Title,
                    PlantName = plantName,
                    DueDate = t.DueDate,
                    Priority = t.Priority,
                    IsCompleted = t.IsCompleted
                });
            }
            return result;
        }

        // Helper method to convert PlantHistory to ActivityItems (avoids expression tree issues)
        private List<global::VEGIBOT.Models.ActivityItem> MapHistoryToActivityItems(List<global::VEGIBOT.Models.PlantHistoryEntry> history)
        {
            var result = new List<global::VEGIBOT.Models.ActivityItem>();
            foreach (var h in history)
            {
                // Map HistoryActivityType to ActivityType
                global::VEGIBOT.Models.ActivityType activityType;
                switch (h.ActivityType)
                {
                    case global::VEGIBOT.Models.HistoryActivityType.Watered:
                        activityType = global::VEGIBOT.Models.ActivityType.Watered;
                        break;
                    case global::VEGIBOT.Models.HistoryActivityType.Fertilized:
                        activityType = global::VEGIBOT.Models.ActivityType.Fertilized;
                        break;
                    case global::VEGIBOT.Models.HistoryActivityType.Pruned:
                        activityType = global::VEGIBOT.Models.ActivityType.Inspected;
                        break;
                    case global::VEGIBOT.Models.HistoryActivityType.TaskCompleted:
                        activityType = global::VEGIBOT.Models.ActivityType.TaskCompleted;
                        break;
                    case global::VEGIBOT.Models.HistoryActivityType.PlantAdded:
                        activityType = global::VEGIBOT.Models.ActivityType.PlantAdded;
                        break;
                    case global::VEGIBOT.Models.HistoryActivityType.Harvested:
                        activityType = global::VEGIBOT.Models.ActivityType.Harvested;
                        break;
                    default:
                        activityType = global::VEGIBOT.Models.ActivityType.Inspected;
                        break;
                }
                result.Add(new global::VEGIBOT.Models.ActivityItem
                {
                    Type = activityType,
                    Description = h.Description,
                    PlantName = h.Plant?.Name ?? "Unknown",
                    Timestamp = h.OccurredAt
                });
            }
            return result;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new global::VEGIBOT.Models.ErrorViewModel
            {
                RequestId = HttpContext.TraceIdentifier
            });
        }

        static string GetGreeting()
        {
            var hour = DateTime.Now.Hour;
            return hour switch
            {
                < 12 => "Good morning",
                < 17 => "Good afternoon",
                < 21 => "Good evening",
                _ => "Good night"
            };
        }
    }
}
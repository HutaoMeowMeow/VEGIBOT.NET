using VEGIBOT.Models;

namespace VEGIBOT.Services
{
    /// <summary>
    /// Provides realistic demonstration data for the dashboard.
    /// All records are clearly labelled as demo data.
    /// Replace method implementations with database-backed calls when ready.
    /// </summary>
    public class DashboardDemoService
    {
        // ── Greeting ──────────────────────────────────────────
        public static string GetGreeting()
        {
            var hour = DateTime.Now.Hour;
            return hour switch
            {
                < 12 => "Good morning",
                < 17 => "Good afternoon",
                < 21 => "Good evening",
                _    => "Good night"
            };
        }

        // ── Plants ────────────────────────────────────────────
        public static IReadOnlyList<PlantCard> GetDemoPlants() =>
        [
            new PlantCard
            {
                Id = 1,
                Name = "Tomato",
                Variety = "Cherry Tomato",
                IconEmoji = "🍅",
                ColorAccent = "#C0392B",
                PlantedDate = DateTime.Now.AddDays(-42),
                Stage = GrowthStage.Fruiting,
                Health = HealthStatus.Excellent,
                GrowthPercent = 78,
                Notes = "Growing vigorously. First fruits turning red."
            },
            new PlantCard
            {
                Id = 2,
                Name = "Lettuce",
                Variety = "Butterhead",
                IconEmoji = "🥬",
                ColorAccent = "#27AE60",
                PlantedDate = DateTime.Now.AddDays(-21),
                Stage = GrowthStage.Vegetative,
                Health = HealthStatus.Good,
                GrowthPercent = 52,
                Notes = "Leaves are full and crisp."
            },
            new PlantCard
            {
                Id = 3,
                Name = "Eggplant",
                Variety = "Black Beauty",
                IconEmoji = "🍆",
                ColorAccent = "#7D3C98",
                PlantedDate = DateTime.Now.AddDays(-56),
                Stage = GrowthStage.Flowering,
                Health = HealthStatus.Fair,
                GrowthPercent = 63,
                Notes = "Flowering well. Watch for spider mites."
            },
            new PlantCard
            {
                Id = 4,
                Name = "Bell Pepper",
                Variety = "California Wonder",
                IconEmoji = "🫑",
                ColorAccent = "#D4AC0D",
                PlantedDate = DateTime.Now.AddDays(-35),
                Stage = GrowthStage.Fruiting,
                Health = HealthStatus.Good,
                GrowthPercent = 70,
                Notes = "Peppers forming. Keep soil consistently moist."
            }
        ];

        // ── Care Tasks ────────────────────────────────────────
        public static IReadOnlyList<CareTask> GetDemoTasks() =>
        [
            new CareTask
            {
                Id = 1,
                Title = "Water Tomato Plant",
                PlantName = "Tomato",
                DueDate = DateTime.Today,
                Priority = TaskPriority.High,
                IsCompleted = false
            },
            new CareTask
            {
                Id = 2,
                Title = "Fertilize Lettuce",
                PlantName = "Lettuce",
                DueDate = DateTime.Today.AddDays(1),
                Priority = TaskPriority.Medium,
                IsCompleted = false
            },
            new CareTask
            {
                Id = 3,
                Title = "Inspect Eggplant Leaves",
                PlantName = "Eggplant",
                DueDate = DateTime.Today,
                Priority = TaskPriority.Urgent,
                IsCompleted = false
            },
            new CareTask
            {
                Id = 4,
                Title = "Check Bell Pepper Soil",
                PlantName = "Bell Pepper",
                DueDate = DateTime.Today.AddDays(2),
                Priority = TaskPriority.Low,
                IsCompleted = true
            },
            new CareTask
            {
                Id = 5,
                Title = "Prune Tomato Side Shoots",
                PlantName = "Tomato",
                DueDate = DateTime.Today.AddDays(3),
                Priority = TaskPriority.Medium,
                IsCompleted = false
            }
        ];

        // ── Stats ─────────────────────────────────────────────
        public static IReadOnlyList<StatCard> GetDemoStats(
            IReadOnlyList<PlantCard> plants,
            IReadOnlyList<CareTask> tasks)
        {
            var totalPlants   = plants.Count;
            var healthyPlants = plants.Count(p =>
                p.Health is HealthStatus.Excellent or HealthStatus.Good);
            var pendingTasks     = tasks.Count(t => !t.IsCompleted);
            var upcomingReminders = tasks.Count(t =>
                !t.IsCompleted && t.DueDate <= DateTime.Today.AddDays(3));

            return
            [
                new StatCard
                {
                    Label      = "Total Plants",
                    Value      = totalPlants.ToString(),
                    ColorClass = "stat-green",
                    Trend      = "+1 this week",
                    IconSvgPath =
                        "M12 2C8 6 5 10 5 14a7 7 0 0 0 14 0c0-4-3-8-7-12z" +
                        " M12 14v6 M12 12l3-3 M12 14l-3-3"
                },
                new StatCard
                {
                    Label      = "Healthy Plants",
                    Value      = healthyPlants.ToString(),
                    ColorClass = "stat-teal",
                    Trend      = $"{healthyPlants * 100 / Math.Max(totalPlants, 1)}% healthy",
                    IconSvgPath =
                        "M22 11.08V12a10 10 0 1 1-5.93-9.14" +
                        " M22 4 12 14.01l-3-3"
                },
                new StatCard
                {
                    Label      = "Pending Tasks",
                    Value      = pendingTasks.ToString(),
                    ColorClass = "stat-amber",
                    Trend      = "Due today & soon",
                    IconSvgPath =
                        "M9 11l3 3L22 4" +
                        " M21 12v7a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h11"
                },
                new StatCard
                {
                    Label      = "Upcoming Reminders",
                    Value      = upcomingReminders.ToString(),
                    ColorClass = "stat-rose",
                    Trend      = "Next 3 days",
                    IconSvgPath =
                        "M18 8A6 6 0 0 0 6 8c0 7-3 9-3 9h18s-3-2-3-9" +
                        " M13.73 21a2 2 0 0 1-3.46 0"
                }
            ];
        }

        // ── Weather (demo) ────────────────────────────────────
        public static WeatherDemo GetDemoWeather() => new()
        {
            Condition          = "Partly Cloudy",
            ConditionIcon      = "⛅",
            TemperatureCelsius = 24,
            HumidityPercent    = 62,
            WindKph            = 14,
            GardeningTip       = "Good day for watering in the morning. " +
                                 "Avoid afternoon sun on new seedlings.",
            IsDemoData         = true
        };

        // ── Activity ──────────────────────────────────────────
        public static IReadOnlyList<ActivityItem> GetDemoActivity() =>
        [
            new ActivityItem
            {
                Type        = ActivityType.PlantAdded,
                Description = "Added Bell Pepper to your garden",
                PlantName   = "Bell Pepper",
                Timestamp   = DateTime.Now.AddDays(-35),
                IconEmoji   = "🌱"
            },
            new ActivityItem
            {
                Type        = ActivityType.Watered,
                Description = "Watered Tomato plant",
                PlantName   = "Tomato",
                Timestamp   = DateTime.Now.AddHours(-3),
                IconEmoji   = "💧"
            },
            new ActivityItem
            {
                Type        = ActivityType.Fertilized,
                Description = "Applied liquid fertilizer to Lettuce",
                PlantName   = "Lettuce",
                Timestamp   = DateTime.Now.AddDays(-2),
                IconEmoji   = "🧪"
            },
            new ActivityItem
            {
                Type        = ActivityType.ReminderCompleted,
                Description = "Completed: Check Bell Pepper soil",
                PlantName   = "Bell Pepper",
                Timestamp   = DateTime.Now.AddDays(-1),
                IconEmoji   = "✅"
            },
            new ActivityItem
            {
                Type        = ActivityType.Inspected,
                Description = "Inspected Eggplant for pests",
                PlantName   = "Eggplant",
                Timestamp   = DateTime.Now.AddHours(-18),
                IconEmoji   = "🔍"
            }
        ];

        // ── Insights ──────────────────────────────────────────
        public static IReadOnlyList<GardenInsight> GetInsights() =>
        [
            new GardenInsight
            {
                Title    = "Consistent Watering",
                Body     = "Most vegetables prefer deep, infrequent watering over shallow daily irrigation. " +
                           "Aim for 2–3 times per week depending on weather.",
                IconEmoji = "💧",
                TagLabel  = "Watering"
            },
            new GardenInsight
            {
                Title    = "Tomato Care",
                Body     = "Remove side shoots (suckers) from indeterminate tomato varieties regularly " +
                           "to focus energy into fruit development.",
                IconEmoji = "🍅",
                TagLabel  = "Pruning"
            },
            new GardenInsight
            {
                Title    = "Soil Health",
                Body     = "Test soil pH before the growing season. Most vegetables thrive in a " +
                           "pH range of 6.0–7.0. Amend with compost to improve structure and drainage.",
                IconEmoji = "🌍",
                TagLabel  = "Soil"
            }
        ];

        // ── Build the full view-model ─────────────────────────
        public static DashboardViewModel BuildViewModel(string userName)
        {
            var plants   = GetDemoPlants();
            var tasks    = GetDemoTasks();
            var stats    = GetDemoStats(plants, tasks);
            var weather  = GetDemoWeather();
            var activity = GetDemoActivity();
            var insights = GetInsights();

            return new DashboardViewModel
            {
                UserName       = userName,
                Greeting       = GetGreeting(),
                Now            = DateTime.Now,
                IsDemoData     = true,
                Stats          = stats,
                Plants         = plants,
                Tasks          = tasks,
                Weather        = weather,
                RecentActivity = activity,
                Insights       = insights
            };
        }
    }
}

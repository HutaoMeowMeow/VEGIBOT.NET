namespace VEGIBOT.Models
{
    // ─── Enumerations ──────────────────────────────────────────
    public enum GrowthStage
    {
        Seedling,
        Vegetative,
        Flowering,
        Fruiting,
        Mature,
        Harvesting
    }

    public enum HealthStatus
    {
        Excellent,
        Good,
        Fair,
        NeedsAttention,
        Critical
    }

    public enum TaskPriority
    {
        Low,
        Medium,
        High,
        Urgent
    }

    public enum ActivityType
    {
        PlantAdded,
        Watered,
        Fertilized,
        ReminderCompleted,
        TaskCompleted,
        Inspected,
        Harvested
    }

    // ─── Plant card displayed on dashboard ─────────────────────
    public class PlantCard
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Variety { get; set; } = string.Empty;
        public string IconEmoji { get; set; } = "🌱";
        public string ColorAccent { get; set; } = "#3F7651";   // hex for CSS
        public DateTime PlantedDate { get; set; }
        public GrowthStage Stage { get; set; }
        public HealthStatus Health { get; set; }
        /// <summary>0–100 growth progress for the progress bar</summary>
        public int GrowthPercent { get; set; }
        public string? Notes { get; set; }
    }

    // ─── Care task ─────────────────────────────────────────────
    public class CareTask
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string PlantName { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public TaskPriority Priority { get; set; }
        public bool IsCompleted { get; set; }
    }

    // ─── Stat card ─────────────────────────────────────────────
    public class StatCard
    {
        public string Label { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string IconSvgPath { get; set; } = string.Empty;
        public string ColorClass { get; set; } = string.Empty;
        public string? Trend { get; set; }
    }

    // ─── Weather (demo only) ───────────────────────────────────
    public class WeatherDemo
    {
        public string Condition { get; set; } = "Partly Cloudy";
        public string ConditionIcon { get; set; } = "⛅";
        public int TemperatureCelsius { get; set; } = 24;
        public int HumidityPercent { get; set; } = 62;
        public int WindKph { get; set; } = 14;
        public string GardeningTip { get; set; } = string.Empty;
        public bool IsDemoData { get; set; } = true;
    }

    // ─── Activity feed item ────────────────────────────────────
    public class ActivityItem
    {
        public ActivityType Type { get; set; }
        public string Description { get; set; } = string.Empty;
        public string PlantName { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string IconEmoji { get; set; } = "📌";
    }

    // ─── Garden insight/tip ────────────────────────────────────
    public class GardenInsight
    {
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string IconEmoji { get; set; } = "💡";
        public string TagLabel { get; set; } = string.Empty;
    }

    // ─── The full dashboard view-model ─────────────────────────
    public class DashboardViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public string Greeting { get; set; } = string.Empty;    // "Good morning", etc.
        public DateTime Now { get; set; } = DateTime.Now;
        public bool IsDemoData { get; set; } = true;

        public IReadOnlyList<StatCard> Stats { get; set; } = [];
        public IReadOnlyList<PlantCard> Plants { get; set; } = [];
        public IReadOnlyList<CareTask> Tasks { get; set; } = [];
        public WeatherDemo Weather { get; set; } = new();
        public IReadOnlyList<ActivityItem> RecentActivity { get; set; } = [];
        public IReadOnlyList<GardenInsight> Insights { get; set; } = [];
    }
}

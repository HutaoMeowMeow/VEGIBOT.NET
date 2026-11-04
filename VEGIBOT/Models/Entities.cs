using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VEGIBOT.Models
{
    // ══════════════════════════════════════════════════════════
    // PLANT
    // ══════════════════════════════════════════════════════════
    public class Plant
    {
        public int Id { get; set; }

        [Required] public string UserId { get; set; } = string.Empty;

        [Required, MaxLength(100)] public string Name { get; set; } = string.Empty;
        [MaxLength(100)] public string? VegetableType { get; set; }
        [MaxLength(100)] public string? Variety { get; set; }

        public DateTime PlantedDate { get; set; } = DateTime.Today;
        public DateTime? ExpectedHarvestDate { get; set; }

        public GrowthStage Stage { get; set; } = GrowthStage.Seedling;
        public HealthStatus Health { get; set; } = HealthStatus.Good;

        [MaxLength(50)] public string? WateringFrequency { get; set; }
        [MaxLength(100)] public string? SoilType { get; set; }
        [MaxLength(100)] public string? SunlightRequirements { get; set; }
        [MaxLength(1000)] public string? Notes { get; set; }
        [MaxLength(10)] public string IconEmoji { get; set; } = "🌱";
        [MaxLength(10)] public string ColorAccent { get; set; } = "#3F7651";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<PlantTask> Tasks { get; set; } = [];
        public ICollection<Reminder> Reminders { get; set; } = [];
        public ICollection<PlantHistoryEntry> History { get; set; } = [];
    }

    // ══════════════════════════════════════════════════════════
    // TASK CATEGORY
    // ══════════════════════════════════════════════════════════
    public enum TaskCategory
    {
        Watering,
        Fertilizing,
        Pruning,
        PestInspection,
        Repotting,
        GeneralCare
    }

    // ══════════════════════════════════════════════════════════
    // PLANT TASK
    // ══════════════════════════════════════════════════════════
    public class PlantTask
    {
        public int Id { get; set; }

        [Required] public string UserId { get; set; } = string.Empty;

        public int? PlantId { get; set; }
        public Plant? Plant { get; set; }

        [Required, MaxLength(200)] public string Title { get; set; } = string.Empty;
        [MaxLength(1000)] public string? Description { get; set; }

        public TaskCategory Category { get; set; } = TaskCategory.GeneralCare;
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;

        public DateTime DueDate { get; set; } = DateTime.Today;
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    // ══════════════════════════════════════════════════════════
    // RECURRENCE
    // ══════════════════════════════════════════════════════════
    public enum RecurrenceType
    {
        None,
        Daily,
        EveryTwoDays,
        Weekly,
        EveryTwoWeeks,
        Monthly,
        Custom
    }

    // ═══════════════════════════════════════════════════════════
    // REMINDER TYPE
    // ══════════════════════════════════════════════════════════
    public enum ReminderType
    {
        Watering,
        Fertilizing,
        Pruning,
        PestInspection,
        Repotting,
        Harvesting,
        Other
    }

    // ══════════════════════════════════════════════════════════
    // REMINDER
    // ══════════════════════════════════════════════════════════
    public class Reminder
    {
        public int Id { get; set; }

        [Required] public string UserId { get; set; } = string.Empty;

        public int? PlantId { get; set; }
        public Plant? Plant { get; set; }

        [Required, MaxLength(200)] public string Title { get; set; } = string.Empty;
        [MaxLength(1000)] public string? Notes { get; set; }

        public DateTime ReminderDate { get; set; } = DateTime.Today.AddDays(1);
        public TimeSpan ReminderTime { get; set; } = new TimeSpan(8, 0, 0);

        public bool IsEnabled { get; set; } = true;
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? SnoozedUntil { get; set; }

        public RecurrenceType Recurrence { get; set; } = RecurrenceType.None;
        public ReminderType ReminderType { get; set; } = ReminderType.Watering;
        /// <summary>Custom interval in days when Recurrence == Custom.</summary>
        public int? CustomIntervalDays { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    // ══════════════════════════════════════════════════════════
    // PLANT HISTORY
    // ══════════════════════════════════════════════════════════
    public enum HistoryActivityType
    {
        Watered,
        Fertilized,
        Pruned,
        Repotted,
        PestInspection,
        HealthChanged,
        NoteAdded,
        TaskCompleted,
        PlantAdded,
        Harvested,
        StageChanged,
        ReminderEnabled,
        ReminderDisabled
    }

    public class PlantHistoryEntry
    {
        public int Id { get; set; }

        [Required] public string UserId { get; set; } = string.Empty;

        public int? PlantId { get; set; }
        public Plant? Plant { get; set; }

        public HistoryActivityType ActivityType { get; set; }
        [MaxLength(500)] public string Description { get; set; } = string.Empty;
        [MaxLength(1000)] public string? Details { get; set; }

        public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    }

    // ══════════════════════════════════════════════════════════
    // CALENDAR EVENT
    // ══════════════════════════════════════════════════════════
    public class CalendarEvent
    {
        public int Id { get; set; }

        [Required] public string UserId { get; set; } = string.Empty;

        [MaxLength(200)] public string Title { get; set; } = string.Empty;

        public DateTime Date { get; set; }
        public string Type { get; set; } = "task"; // task, reminder
        public string ColorClass { get; set; } = "evt-task";
        public bool IsCompleted { get; set; }
        public bool IsOverdue { get; set; }
        public int? PlantId { get; set; }
        public Plant? Plant { get; set; }

        [MaxLength(100)] public string? PlantName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    // ══════════════════════════════════════════════════════════
    // USER SETTINGS
    // ══════════════════════════════════════════════════════════
    public enum TemperatureUnit { Celsius, Fahrenheit }
    public enum DateDisplayFormat { DMY, MDY, YMD }

    public class UserSettings
    {
        public int Id { get; set; }
        [Required] public string UserId { get; set; } = string.Empty;

        [MaxLength(100)] public string? DisplayName { get; set; }

        public TemperatureUnit TemperatureUnit { get; set; } = TemperatureUnit.Celsius;
        public DateDisplayFormat DateFormat { get; set; } = DateDisplayFormat.DMY;

        public bool NotificationsEnabled { get; set; } = true;
        public bool EmailReminders { get; set; }
        public bool WeeklyDigest { get; set; }

        // Gardening preferences
        [MaxLength(50)] public string? GardenClimate { get; set; }
        [MaxLength(200)] public string? GardenLocation { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}

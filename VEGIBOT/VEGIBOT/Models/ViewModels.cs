using System.ComponentModel.DataAnnotations;

namespace VEGIBOT.Models
{
    // ══════════════════════════════════════════════════════════
    // PLANTS
    // ══════════════════════════════════════════════════════════
    public class PlantFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Plant name is required.")]
        [MaxLength(100)]
        [Display(Name = "Plant Name")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(100)]
        [Display(Name = "Vegetable Type")]
        public string? VegetableType { get; set; }

        [MaxLength(100)]
        [Display(Name = "Variety")]
        public string? Variety { get; set; }

        [Required(ErrorMessage = "Planting date is required.")]
        [Display(Name = "Planting Date")]
        [DataType(DataType.Date)]
        public DateTime PlantedDate { get; set; } = DateTime.Today;

        [Display(Name = "Expected Harvest Date")]
        [DataType(DataType.Date)]
        public DateTime? ExpectedHarvestDate { get; set; }

        [Display(Name = "Growth Stage")]
        public GrowthStage Stage { get; set; } = GrowthStage.Seedling;

        [Display(Name = "Health Status")]
        public HealthStatus Health { get; set; } = HealthStatus.Good;

        [MaxLength(50)]
        [Display(Name = "Watering Frequency")]
        public string? WateringFrequency { get; set; }

        [MaxLength(100)]
        [Display(Name = "Soil Type")]
        public string? SoilType { get; set; }

        [MaxLength(100)]
        [Display(Name = "Sunlight Requirements")]
        public string? SunlightRequirements { get; set; }

        [MaxLength(1000)]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        [MaxLength(10)]
        [Display(Name = "Icon Emoji")]
        public string IconEmoji { get; set; } = "🌱";

        [MaxLength(10)]
        [Display(Name = "Color Accent")]
        public string ColorAccent { get; set; } = "#3F7651";
    }

    public class PlantFilterViewModel
    {
        public string? Search { get; set; }
        public HealthStatus? Health { get; set; }
        public GrowthStage? Stage { get; set; }
        public string SortBy { get; set; } = "name";
        public IReadOnlyList<Plant> Plants { get; set; } = [];
    }

    // ══════════════════════════════════════════════════════════
    // TASKS
    // ══════════════════════════════════════════════════════════
    public class TaskFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [MaxLength(200)]
        [Display(Name = "Task Title")]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Plant")]
        public int? PlantId { get; set; }

        [Display(Name = "Category")]
        public TaskCategory Category { get; set; } = TaskCategory.GeneralCare;

        [Required(ErrorMessage = "Due date is required.")]
        [Display(Name = "Due Date")]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; } = DateTime.Today;

        [Display(Name = "Priority")]
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;

        public bool IsCompleted { get; set; }
    }

    public class TaskListViewModel
    {
        public string Filter { get; set; } = "all"; // all, pending, completed, overdue
        public string? Search { get; set; }
        public IReadOnlyList<PlantTask> Tasks { get; set; } = [];
        public IReadOnlyList<Plant> UserPlants { get; set; } = [];
    }

    // ══════════════════════════════════════════════════════════
    // REMINDERS
    // ══════════════════════════════════════════════════════════
    public class ReminderFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [MaxLength(200)]
        [Display(Name = "Reminder Title")]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        [Display(Name = "Plant")]
        public int? PlantId { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        [Display(Name = "Reminder Date")]
        [DataType(DataType.Date)]
        public DateTime ReminderDate { get; set; } = DateTime.Today.AddDays(1);

        [Required]
        [Display(Name = "Reminder Time")]
        public string ReminderTime { get; set; } = "08:00";

        [Display(Name = "Enabled")]
        public bool IsEnabled { get; set; } = true;

        [Display(Name = "Recurrence")]
        public RecurrenceType Recurrence { get; set; } = RecurrenceType.None;

        [Range(1, 365, ErrorMessage = "Custom interval must be between 1 and 365 days.")]
        [Display(Name = "Custom Interval (days)")]
        public int? CustomIntervalDays { get; set; }
    }

    public class ReminderListViewModel
    {
        public string Filter { get; set; } = "upcoming";
        public IReadOnlyList<Reminder> Reminders { get; set; } = [];
        public IReadOnlyList<Plant> UserPlants { get; set; } = [];
    }

    // ══════════════════════════════════════════════════════════
    // CALENDAR
    // ══════════════════════════════════════════════════════════
    public class CalendarViewModel
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string View { get; set; } = "month"; // month, week, day
        public IReadOnlyList<CalendarEvent> Events { get; set; } = [];
    }

    public class CalendarEvent
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Type { get; set; } = "task"; // task, reminder
        public string ColorClass { get; set; } = "evt-task";
        public bool IsCompleted { get; set; }
        public bool IsOverdue { get; set; }
        public string? PlantName { get; set; }
    }

    // ══════════════════════════════════════════════════════════
    // PLANT HISTORY
    // ══════════════════════════════════════════════════════════
    public class HistoryListViewModel
    {
        public int? PlantId { get; set; }
        public HistoryActivityType? ActivityType { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public IReadOnlyList<PlantHistoryEntry> Entries { get; set; } = [];
        public IReadOnlyList<Plant> UserPlants { get; set; } = [];
    }

    // ══════════════════════════════════════════════════════════
    // PROFILE
    // ══════════════════════════════════════════════════════════
    public class ProfileViewModel
    {
        public string Email { get; set; } = string.Empty;

        [MaxLength(100)]
        [Display(Name = "Display Name")]
        public string? DisplayName { get; set; }

        public DateTime? AccountCreatedAt { get; set; }
        public int TotalPlants { get; set; }
        public int TotalTasks { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Current password is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please confirm your new password.")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        [Display(Name = "Confirm New Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    // ══════════════════════════════════════════════════════════
    // SETTINGS
    // ══════════════════════════════════════════════════════════
    public class SettingsViewModel
    {
        [MaxLength(100)]
        [Display(Name = "Display Name")]
        public string? DisplayName { get; set; }

        [Display(Name = "Temperature Unit")]
        public TemperatureUnit TemperatureUnit { get; set; } = TemperatureUnit.Celsius;

        [Display(Name = "Date Format")]
        public DateDisplayFormat DateFormat { get; set; } = DateDisplayFormat.DMY;

        [Display(Name = "Enable Notifications")]
        public bool NotificationsEnabled { get; set; } = true;

        [Display(Name = "Email Reminders")]
        public bool EmailReminders { get; set; }

        [Display(Name = "Weekly Digest")]
        public bool WeeklyDigest { get; set; }

        [MaxLength(50)]
        [Display(Name = "Garden Climate")]
        public string? GardenClimate { get; set; }

        [MaxLength(200)]
        [Display(Name = "Garden Location")]
        public string? GardenLocation { get; set; }
    }

    // ══════════════════════════════════════════════════════════
    // GARDEN AI
    // ══════════════════════════════════════════════════════════
    public class AiChatMessage
    {
        public string Role { get; set; } = "user"; // user | assistant | system
        public string Content { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }

    public class AiChatViewModel
    {
        public IReadOnlyList<AiChatMessage> Messages { get; set; } = [];
        public IReadOnlyList<Plant> UserPlants { get; set; } = [];
        public bool IsConfigured { get; set; } = false;
        public string? ConfigurationNote { get; set; }
    }
}

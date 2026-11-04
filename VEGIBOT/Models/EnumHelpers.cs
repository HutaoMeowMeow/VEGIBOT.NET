namespace VEGIBOT.Models
{
    public static class EnumHelpers
    {
        public static string DisplayName(this HealthStatus h) => h switch
        {
            HealthStatus.Excellent      => "Excellent",
            HealthStatus.Good           => "Good",
            HealthStatus.Fair           => "Fair",
            HealthStatus.NeedsAttention => "Needs Attention",
            HealthStatus.Critical       => "Critical",
            _                           => h.ToString()
        };

        public static string DisplayName(this GrowthStage s) => s switch
        {
            GrowthStage.Seedling    => "Seedling",
            GrowthStage.Vegetative  => "Vegetative",
            GrowthStage.Flowering   => "Flowering",
            GrowthStage.Fruiting    => "Fruiting",
            GrowthStage.Mature      => "Mature",
            GrowthStage.Harvesting  => "Harvesting",
            _                       => s.ToString()
        };

        public static string DisplayName(this TaskPriority p) => p switch
        {
            TaskPriority.Low    => "Low",
            TaskPriority.Medium => "Medium",
            TaskPriority.High   => "High",
            TaskPriority.Urgent => "Urgent",
            _                   => p.ToString()
        };

        public static string DisplayName(this TaskCategory c) => c switch
        {
            TaskCategory.Watering       => "Watering",
            TaskCategory.Fertilizing    => "Fertilizing",
            TaskCategory.Pruning        => "Pruning",
            TaskCategory.PestInspection => "Pest Inspection",
            TaskCategory.Repotting      => "Repotting",
            TaskCategory.GeneralCare    => "General Care",
            _                           => c.ToString()
        };

        public static string DisplayName(this RecurrenceType r) => r switch
        {
            RecurrenceType.None          => "Does not repeat",
            RecurrenceType.Daily         => "Daily",
            RecurrenceType.EveryTwoDays  => "Every 2 days",
            RecurrenceType.Weekly        => "Weekly",
            RecurrenceType.EveryTwoWeeks => "Every 2 weeks",
            RecurrenceType.Monthly       => "Monthly",
            RecurrenceType.Custom        => "Custom interval",
            _                            => r.ToString()
        };

        public static string HealthCssClass(this HealthStatus h) => h switch
        {
            HealthStatus.Excellent      => "health-excellent",
            HealthStatus.Good           => "health-good",
            HealthStatus.Fair           => "health-fair",
            HealthStatus.NeedsAttention => "health-needs-attention",
            HealthStatus.Critical       => "health-critical",
            _                           => "health-good"
        };

        public static string PriorityCssClass(this TaskPriority p) => p switch
        {
            TaskPriority.Urgent => "priority-urgent",
            TaskPriority.High   => "priority-high",
            TaskPriority.Medium => "priority-medium",
            TaskPriority.Low    => "priority-low",
            _                   => "priority-medium"
        };

        public static string DisplayName(this HistoryActivityType t) => t switch
        {
            HistoryActivityType.Watered       => "Watered",
            HistoryActivityType.Fertilized    => "Fertilized",
            HistoryActivityType.Pruned        => "Pruned",
            HistoryActivityType.Repotted      => "Repotted",
            HistoryActivityType.PestInspection=> "Pest Inspection",
            HistoryActivityType.HealthChanged => "Health Changed",
            HistoryActivityType.NoteAdded     => "Note Added",
            HistoryActivityType.TaskCompleted => "Task Completed",
            HistoryActivityType.PlantAdded    => "Plant Added",
            HistoryActivityType.Harvested     => "Harvested",
            HistoryActivityType.StageChanged  => "Stage Changed",
            _                                 => t.ToString()
        };

        public static string Icon(this HistoryActivityType t) => t switch
        {
            HistoryActivityType.Watered        => "💧",
            HistoryActivityType.Fertilized     => "🧪",
            HistoryActivityType.Pruned         => "✂️",
            HistoryActivityType.Repotted       => "🪴",
            HistoryActivityType.PestInspection => "🔍",
            HistoryActivityType.HealthChanged  => "💊",
            HistoryActivityType.NoteAdded      => "📝",
            HistoryActivityType.TaskCompleted  => "✅",
            HistoryActivityType.PlantAdded     => "🌱",
            HistoryActivityType.Harvested      => "🌾",
            HistoryActivityType.StageChanged   => "📈",
            _                                  => "📌"
        };
    }
}

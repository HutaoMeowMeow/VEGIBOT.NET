using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VEGIBOT.Models;

namespace VEGIBOT.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Plant> Plants => Set<Plant>();
        public DbSet<PlantTask> PlantTasks => Set<PlantTask>();
        public DbSet<Reminder> Reminders => Set<Reminder>();
        public DbSet<PlantHistoryEntry> PlantHistory => Set<PlantHistoryEntry>();
        public DbSet<UserSettings> UserSettings => Set<UserSettings>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Plant>()
                .HasIndex(p => p.UserId);

            builder.Entity<PlantTask>()
                .HasIndex(t => t.UserId);

            builder.Entity<Reminder>()
                .HasIndex(r => r.UserId);

            builder.Entity<PlantHistoryEntry>()
                .HasIndex(h => h.UserId);

            builder.Entity<UserSettings>()
                .HasIndex(s => s.UserId)
                .IsUnique();
        }
    }
}

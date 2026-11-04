using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VEGIBOT.Data;
using VEGIBOT.Models;

namespace VEGIBOT.Controllers
{
    [Authorize]
    public class CalendarController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public CalendarController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        private string UserId() => _userManager.GetUserId(User)!;

        // GET /Calendar?view=month&year=2024&month=1
        public async Task<IActionResult> Index(string view = "month", int year = 0, int month = 0)
        {
            var userId = UserId();
            var now = DateTime.UtcNow;

            // Set default year/month if not provided
            if (year == 0) year = now.Year;
            if (month == 0) month = now.Month;

            // Build monthly view
            if (view == "month")
            {
                var firstOfMonth = new DateTime(year, month, 1);
                var lastOfMonth = firstOfMonth.AddMonths(1).AddDays(-1);

                // Get all events for this user's plants - use Plant history
                var events = await _db.PlantHistory
                    .Where(h => h.UserId == userId && h.OccurredAt >= firstOfMonth && h.OccurredAt <= lastOfMonth)
                    .Select(h => new CalendarEvent
                    {
                        Id = h.Id,
                        Title = h.ActivityType.ToString(),
                        Date = h.OccurredAt,
                        Type = "history",
                        ColorClass = "evt-history",
                        IsCompleted = true,
                        PlantName = h.Plant == null ? "" : h.Plant.Name
                    })
                    .ToListAsync();

                // Also include tasks with due dates in this month
                var taskEvents = await _db.PlantTasks
                    .Where(t => t.UserId == userId && t.DueDate >= firstOfMonth && t.DueDate <= lastOfMonth && !t.IsCompleted)
                    .Select(t => new CalendarEvent
                    {
                        Id = t.Id,
                        Title = t.Title,
                        Date = t.DueDate,
                        Type = "task",
                        ColorClass = "evt-task",
                        IsCompleted = false,
                        IsOverdue = t.DueDate < now,
                        PlantName = t.Plant == null ? "" : t.Plant.Name
                    })
                    .ToListAsync();

                // Include reminders
                var reminderEvents = await _db.Reminders
                    .Where(r => r.UserId == userId && r.ReminderDate >= firstOfMonth && r.ReminderDate <= lastOfMonth && !r.IsCompleted && r.IsEnabled)
                    .Select(r => new CalendarEvent
                    {
                        Id = r.Id,
                        Title = r.Title,
                        Date = r.ReminderDate,
                        Type = "reminder",
                        ColorClass = "evt-reminder",
                        IsCompleted = false,
                        PlantName = r.Plant == null ? "" : r.Plant.Name
                    })
                    .ToListAsync();

                var allEvents = events.Concat(taskEvents).Concat(reminderEvents).ToList();

                var vm = new CalendarViewModel
                {
                    Year = year,
                    Month = month,
                    View = "month",
                    Events = allEvents
                };

                return View(vm);
            }

            // Weekly view
            if (view == "week")
            {
                var targetDate = new DateTime(year, month, 1);
                var monday = targetDate.AddDays(-(int)targetDate.DayOfWeek + (int)DayOfWeek.Monday);
                var sunday = monday.AddDays(6);

                var events = await _db.PlantHistory
                    .Where(h => h.UserId == userId && h.OccurredAt >= monday && h.OccurredAt <= sunday)
                    .Select(h => new CalendarEvent
                    {
                        Id = h.Id,
                        Title = h.ActivityType.ToString(),
                        Date = h.OccurredAt,
                        Type = "history",
                        ColorClass = "evt-history",
                        IsCompleted = true,
                        PlantName = h.Plant == null ? "" : h.Plant.Name
                    })
                    .ToListAsync();

                var taskEvents = await _db.PlantTasks
                    .Where(t => t.UserId == userId && t.DueDate >= monday && t.DueDate <= sunday && !t.IsCompleted)
                    .Select(t => new CalendarEvent
                    {
                        Id = t.Id,
                        Title = t.Title,
                        Date = t.DueDate,
                        Type = "task",
                        ColorClass = "evt-task",
                        IsCompleted = false,
                        IsOverdue = t.DueDate < now,
                        PlantName = t.Plant == null ? "" : t.Plant.Name
                    })
                    .ToListAsync();

                var reminderEvents = await _db.Reminders
                    .Where(r => r.UserId == userId && r.ReminderDate >= monday && r.ReminderDate <= sunday && !r.IsCompleted && r.IsEnabled)
                    .Select(r => new CalendarEvent
                    {
                        Id = r.Id,
                        Title = r.Title,
                        Date = r.ReminderDate,
                        Type = "reminder",
                        ColorClass = "evt-reminder",
                        IsCompleted = false,
                        PlantName = r.Plant == null ? "" : r.Plant.Name
                    })
                    .ToListAsync();

                var allEvents = events.Concat(taskEvents).Concat(reminderEvents).ToList();

                var vm = new CalendarViewModel
                {
                    Year = year,
                    Month = month,
                    View = "week",
                    Events = allEvents
                };

                return View(vm);
            }

            // Daily view
            if (view == "day")
            {
                var targetDate = new DateTime(year, month, 1);

                var events = await _db.PlantHistory
                    .Where(h => h.UserId == userId && h.OccurredAt == targetDate)
                    .Select(h => new CalendarEvent
                    {
                        Id = h.Id,
                        Title = h.ActivityType.ToString(),
                        Date = h.OccurredAt,
                        Type = "history",
                        ColorClass = "evt-history",
                        IsCompleted = true,
                        PlantName = h.Plant == null ? "" : h.Plant.Name
                    })
                    .ToListAsync();

                var vm = new CalendarViewModel
                {
                    Year = year,
                    Month = month,
                    View = "day",
                    Events = events
                };

                return View(vm);
            }

            return View();
        }

        // GET /Calendar/Create?plantId=5&date=2024-01-15
        public async Task<IActionResult> Create(int? plantId = null, string? date = null)
        {
            var plant = plantId.HasValue ? await _db.Plants.FindAsync(plantId.Value) : null;
            var vm = new CalendarEvent
            {
                Date = date != null ? DateTime.Parse(date) : DateTime.Today,
                Title = plant == null ? "New Event" : plant.Name + " Care",
                PlantName = plant == null ? "" : plant.Name
            };
            return View(vm);
        }

        // POST /Calendar/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CalendarEvent evt)
        {
            if (!ModelState.IsValid) return View(evt);

            evt.UserId = UserId();
            _db.CalendarEvents.Add(evt);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Calendar event created.";
            return RedirectToAction(nameof(Index), new { view = "month", year = evt.Date.Year, month = evt.Date.Month });
        }

        // GET /Calendar/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var evt = await _db.CalendarEvents.FindAsync(id);
            if (evt == null) return NotFound();
            return View(evt);
        }

        // POST /Calendar/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CalendarEvent evt)
        {
            if (id != evt.Id) return BadRequest();

            evt.UserId = UserId();
            _db.Entry(evt).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            TempData["Success"] = "Calendar event updated.";
            return RedirectToAction(nameof(Index), new { view = "month", year = evt.Date.Year, month = evt.Date.Month });
        }

        // POST /Calendar/Delete/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var evt = await _db.CalendarEvents.FindAsync(id);
            if (evt == null) return NotFound();

            evt.UserId = UserId();
            _db.CalendarEvents.Remove(evt);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Calendar event deleted.";
            return RedirectToAction(nameof(Index), new { view = "month" });
        }
    }
}
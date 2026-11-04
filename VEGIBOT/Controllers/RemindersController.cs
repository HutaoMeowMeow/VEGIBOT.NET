using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VEGIBOT.Data;
using VEGIBOT.Models;

namespace VEGIBOT.Controllers
{
    [Authorize]
    public class RemindersController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public RemindersController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        private string UserId() => _userManager.GetUserId(User)!;

        // Helper to convert string "hh:mm" to TimeSpan
        private static TimeSpan? ParseTimeSpan(string? timeStr)
        {
            if (string.IsNullOrEmpty(timeStr)) return null;
            var parts = timeStr.Split(':');
            if (parts.Length == 2 && int.TryParse(parts[0], out var hour) && int.TryParse(parts[1], out var minute))
                return new TimeSpan(hour, minute, 0);
            return null;
        }

        // Helper to convert TimeSpan to string "hh:mm"
        private static string FormatTimeSpan(TimeSpan? timeSpan)
        {
            if (!timeSpan.HasValue) return "08:00";
            return $"{timeSpan.Value.Hours:D2}:{timeSpan.Value.Minutes:D2}";
        }

        // GET /Reminders
        public async Task<IActionResult> Index(string filter = "all", string? search = null)
        {
            var query = _db.Reminders.Include(r => r.Plant).Where(r => r.UserId == UserId());

            query = filter switch
            {
                "enabled"   => query.Where(r => r.IsEnabled),
                "disabled"  => query.Where(r => !r.IsEnabled),
                "completed" => query.Where(r => r.IsCompleted),
                _           => query
            };

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(r => r.Title.Contains(search));

            var vm = new ReminderListViewModel
            {
                Filter = filter,
                Search = search,
                Reminders = await query.OrderByDescending(r => r.ReminderDate).ToListAsync(),
                UserPlants = await _db.Plants.Where(p => p.UserId == UserId()).OrderBy(p => p.Name).ToListAsync()
            };
            return View(vm);
        }

        // GET /Reminders/Create
        public async Task<IActionResult> Create(int? plantId = null)
        {
            ViewBag.Plants = await _db.Plants.Where(p => p.UserId == UserId()).OrderBy(p => p.Name).ToListAsync();
            return View(new ReminderFormViewModel
            {
                ReminderDate = DateTime.Today.AddDays(1),
                ReminderTime = FormatTimeSpan(null),
                PlantId = plantId,
                Recurrence = RecurrenceType.None,
                ReminderType = ReminderType.Watering,
                IsEnabled = true
            });
        }

        // POST /Reminders/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReminderFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Plants = await _db.Plants.Where(p => p.UserId == UserId()).OrderBy(p => p.Name).ToListAsync();
                return View(vm);
            }

            var reminder = new Reminder
            {
                UserId = UserId(),
                PlantId = vm.PlantId,
                Title = vm.Title,
                Notes = vm.Notes,
                ReminderDate = vm.ReminderDate,
                ReminderTime = ParseTimeSpan(vm.ReminderTime) ?? new TimeSpan(8, 0, 0),
                IsEnabled = vm.IsEnabled,
                IsCompleted = vm.IsCompleted,
                Recurrence = vm.Recurrence,
                CustomIntervalDays = vm.CustomIntervalDays,
                ReminderType = vm.ReminderType,
                CreatedAt = DateTime.UtcNow
            };

            _db.Reminders.Add(reminder);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Reminder created.";
            return RedirectToAction(nameof(Index));
        }

        // GET /Reminders/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var reminder = await _db.Reminders.FirstOrDefaultAsync(r => r.Id == id && r.UserId == UserId());
            if (reminder == null) return NotFound();

            ViewBag.Plants = await _db.Plants.Where(p => p.UserId == UserId()).OrderBy(p => p.Name).ToListAsync();
            return View(new ReminderFormViewModel
            {
                Id = reminder.Id,
                Title = reminder.Title,
                Notes = reminder.Notes,
                ReminderDate = reminder.ReminderDate,
                ReminderTime = FormatTimeSpan(reminder.ReminderTime),
                IsEnabled = reminder.IsEnabled,
                IsCompleted = reminder.IsCompleted,
                Recurrence = reminder.Recurrence,
                CustomIntervalDays = reminder.CustomIntervalDays,
                ReminderType = reminder.ReminderType,
                PlantId = reminder.PlantId
            });
        }

        // POST /Reminders/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ReminderFormViewModel vm)
        {
            if (id != vm.Id) return BadRequest();
            if (!ModelState.IsValid) return View(vm);

            var reminder = await _db.Reminders.FirstOrDefaultAsync(r => r.Id == id && r.UserId == UserId());
            if (reminder == null) return NotFound();

            var oldEnabled = reminder.IsEnabled;

            reminder.Title = vm.Title;
            reminder.Notes = vm.Notes;
            reminder.ReminderDate = vm.ReminderDate;
            reminder.ReminderTime = ParseTimeSpan(vm.ReminderTime) ?? reminder.ReminderTime;
            reminder.IsEnabled = vm.IsEnabled;
            reminder.IsCompleted = vm.IsCompleted;
            reminder.Recurrence = vm.Recurrence;
            reminder.CustomIntervalDays = vm.CustomIntervalDays;
            reminder.ReminderType = vm.ReminderType;

            if (oldEnabled != vm.IsEnabled)
                _db.PlantHistory.Add(new PlantHistoryEntry
                {
                    UserId = UserId(),
                    PlantId = reminder.PlantId,
                    ActivityType = vm.IsEnabled ? HistoryActivityType.ReminderEnabled : HistoryActivityType.ReminderDisabled,
                    Description = vm.IsEnabled ? "Reminder re-enabled" : "Reminder disabled",
                    OccurredAt = DateTime.UtcNow
                });

            await _db.SaveChangesAsync();
            TempData["Success"] = "Reminder updated.";
            return RedirectToAction(nameof(Index));
        }

        // POST /Reminders/Toggle/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(int id)
        {
            var reminder = await _db.Reminders.FirstOrDefaultAsync(r => r.Id == id && r.UserId == UserId());
            if (reminder == null) return NotFound();

            reminder.IsEnabled = !reminder.IsEnabled;
            reminder.SnoozedUntil = reminder.IsEnabled ? null : DateTime.UtcNow.AddHours(24);

            if (!reminder.IsEnabled)
                _db.PlantHistory.Add(new PlantHistoryEntry
                {
                    UserId = UserId(),
                    PlantId = reminder.PlantId,
                    ActivityType = HistoryActivityType.ReminderDisabled,
                    Description = "Reminder snoozed",
                    OccurredAt = DateTime.UtcNow
                });

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST /Reminders/Snooze/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Snooze(int id)
        {
            var reminder = await _db.Reminders.FirstOrDefaultAsync(r => r.Id == id && r.UserId == UserId());
            if (reminder == null) return NotFound();

            reminder.SnoozedUntil = DateTime.UtcNow.AddDays(2);
            reminder.IsEnabled = false;

            await _db.SaveChangesAsync();
            TempData["Success"] = "Reminder snoozed for 2 days.";
            return RedirectToAction(nameof(Index));
        }

        // POST /Reminders/Delete/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var reminder = await _db.Reminders.FirstOrDefaultAsync(r => r.Id == id && r.UserId == UserId());
            if (reminder == null) return NotFound();

            _db.Reminders.Remove(reminder);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Reminder deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}
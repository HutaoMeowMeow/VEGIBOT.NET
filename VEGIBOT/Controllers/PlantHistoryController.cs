using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VEGIBOT.Data;
using VEGIBOT.Models;

namespace VEGIBOT.Controllers
{
    [Authorize]
    public class PlantHistoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public PlantHistoryController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        private string UserId() => _userManager.GetUserId(User)!;

        // GET /PlantHistory
        public async Task<IActionResult> Index(string? filter = "all", string? search = null, DateTime? from = null, DateTime? to = null)
        {
            var userId = UserId();
            var query = _db.PlantHistory.Where(h => h.UserId == userId);

            // Filter by activity type
            query = filter switch
            {
                "watered" => query.Where(h => h.ActivityType == HistoryActivityType.Watered),
                "fertilized" => query.Where(h => h.ActivityType == HistoryActivityType.Fertilized),
                "pruned" => query.Where(h => h.ActivityType == HistoryActivityType.Pruned),
                "harvested" => query.Where(h => h.ActivityType == HistoryActivityType.Harvested),
                "taskcompleted" => query.Where(h => h.ActivityType == HistoryActivityType.TaskCompleted),
                "plantadded" => query.Where(h => h.ActivityType == HistoryActivityType.PlantAdded),
                "healthchanged" => query.Where(h => h.ActivityType == HistoryActivityType.HealthChanged),
                "stagechanged" => query.Where(h => h.ActivityType == HistoryActivityType.StageChanged),
                "reminderenabled" => query.Where(h => h.ActivityType == HistoryActivityType.ReminderEnabled),
                "reminderdisabled" => query.Where(h => h.ActivityType == HistoryActivityType.ReminderDisabled),
                _ => query
            };

            // Filter by plant
            if (filter == "plant" && !string.IsNullOrWhiteSpace(search))
            {
                var plant = await _db.Plants.FirstOrDefaultAsync(p => p.UserId == userId && p.Name == search);
                if (plant != null)
                {
                    query = query.Where(h => h.PlantId == plant.Id);
                }
            }

            // Filter by date range
            if (from.HasValue)
                query = query.Where(h => h.OccurredAt >= from.Value);
            if (to.HasValue)
                query = query.Where(h => h.OccurredAt <= to.Value);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(h => h.Description.Contains(search));

            var vm = new HistoryListViewModel
            {
                Filter = filter ?? "all",
                Search = search,
                From = from,
                To = to,
                Entries = await query.OrderByDescending(h => h.OccurredAt).ToListAsync(),
                UserPlants = await _db.Plants.Where(p => p.UserId == userId).OrderBy(p => p.Name).ToListAsync()
            };
            return View(vm);
        }

        // GET /PlantHistory/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var entry = await _db.PlantHistory
                .Include(h => h.Plant)
                .FirstOrDefaultAsync(h => h.Id == id && h.UserId == UserId());

            if (entry == null) return NotFound();
            return View(entry);
        }
    }
}
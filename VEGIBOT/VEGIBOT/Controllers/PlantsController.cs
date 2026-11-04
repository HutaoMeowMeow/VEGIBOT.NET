using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VEGIBOT.Data;
using VEGIBOT.Models;

namespace VEGIBOT.Controllers
{
    [Authorize]
    public class PlantsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public PlantsController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        private string UserId() => _userManager.GetUserId(User)!;

        // GET /Plants
        public async Task<IActionResult> Index(string? search, HealthStatus? health, GrowthStage? stage, string sortBy = "name")
        {
            var query = _db.Plants.Where(p => p.UserId == UserId());

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p => p.Name.Contains(search) || (p.Variety != null && p.Variety.Contains(search)));

            if (health.HasValue) query = query.Where(p => p.Health == health.Value);
            if (stage.HasValue)  query = query.Where(p => p.Stage == stage.Value);

            query = sortBy switch
            {
                "date"   => query.OrderByDescending(p => p.PlantedDate),
                "health" => query.OrderBy(p => p.Health),
                "stage"  => query.OrderBy(p => p.Stage),
                _        => query.OrderBy(p => p.Name)
            };

            var vm = new PlantFilterViewModel
            {
                Search  = search,
                Health  = health,
                Stage   = stage,
                SortBy  = sortBy,
                Plants  = await query.ToListAsync()
            };
            return View(vm);
        }

        // GET /Plants/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var plant = await _db.Plants
                .Include(p => p.Tasks)
                .Include(p => p.Reminders)
                .Include(p => p.History.OrderByDescending(h => h.OccurredAt).Take(10))
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == UserId());

            if (plant == null) return NotFound();
            return View(plant);
        }

        // GET /Plants/Create
        public IActionResult Create() => View(new PlantFormViewModel { PlantedDate = DateTime.Today });

        // POST /Plants/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PlantFormViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var plant = new Plant
            {
                UserId               = UserId(),
                Name                 = vm.Name,
                VegetableType        = vm.VegetableType,
                Variety              = vm.Variety,
                PlantedDate          = vm.PlantedDate,
                ExpectedHarvestDate  = vm.ExpectedHarvestDate,
                Stage                = vm.Stage,
                Health               = vm.Health,
                WateringFrequency    = vm.WateringFrequency,
                SoilType             = vm.SoilType,
                SunlightRequirements = vm.SunlightRequirements,
                Notes                = vm.Notes,
                IconEmoji            = vm.IconEmoji,
                ColorAccent          = vm.ColorAccent,
                CreatedAt            = DateTime.UtcNow,
                UpdatedAt            = DateTime.UtcNow
            };

            _db.Plants.Add(plant);

            _db.PlantHistory.Add(new PlantHistoryEntry
            {
                UserId       = UserId(),
                PlantId      = plant.Id,
                ActivityType = HistoryActivityType.PlantAdded,
                Description  = $"Added {plant.Name} to your garden",
                OccurredAt   = DateTime.UtcNow
            });

            await _db.SaveChangesAsync();

            // History entry needs the plant Id set after SaveChanges
            var history = await _db.PlantHistory.OrderByDescending(h => h.Id).FirstAsync(h => h.UserId == UserId() && h.ActivityType == HistoryActivityType.PlantAdded);
            if (history.PlantId == 0) { history.PlantId = plant.Id; await _db.SaveChangesAsync(); }

            TempData["Success"] = $"{plant.Name} added to your garden!";
            return RedirectToAction(nameof(Index));
        }

        // GET /Plants/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var plant = await _db.Plants.FirstOrDefaultAsync(p => p.Id == id && p.UserId == UserId());
            if (plant == null) return NotFound();

            var vm = new PlantFormViewModel
            {
                Id                   = plant.Id,
                Name                 = plant.Name,
                VegetableType        = plant.VegetableType,
                Variety              = plant.Variety,
                PlantedDate          = plant.PlantedDate,
                ExpectedHarvestDate  = plant.ExpectedHarvestDate,
                Stage                = plant.Stage,
                Health               = plant.Health,
                WateringFrequency    = plant.WateringFrequency,
                SoilType             = plant.SoilType,
                SunlightRequirements = plant.SunlightRequirements,
                Notes                = plant.Notes,
                IconEmoji            = plant.IconEmoji,
                ColorAccent          = plant.ColorAccent
            };
            return View(vm);
        }

        // POST /Plants/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PlantFormViewModel vm)
        {
            if (id != vm.Id) return BadRequest();
            if (!ModelState.IsValid) return View(vm);

            var plant = await _db.Plants.FirstOrDefaultAsync(p => p.Id == id && p.UserId == UserId());
            if (plant == null) return NotFound();

            var oldHealth = plant.Health;
            var oldStage  = plant.Stage;

            plant.Name                 = vm.Name;
            plant.VegetableType        = vm.VegetableType;
            plant.Variety              = vm.Variety;
            plant.PlantedDate          = vm.PlantedDate;
            plant.ExpectedHarvestDate  = vm.ExpectedHarvestDate;
            plant.Stage                = vm.Stage;
            plant.Health               = vm.Health;
            plant.WateringFrequency    = vm.WateringFrequency;
            plant.SoilType             = vm.SoilType;
            plant.SunlightRequirements = vm.SunlightRequirements;
            plant.Notes                = vm.Notes;
            plant.IconEmoji            = vm.IconEmoji;
            plant.ColorAccent          = vm.ColorAccent;
            plant.UpdatedAt            = DateTime.UtcNow;

            if (oldHealth != vm.Health)
                _db.PlantHistory.Add(new PlantHistoryEntry { UserId = UserId(), PlantId = plant.Id, ActivityType = HistoryActivityType.HealthChanged, Description = $"Health changed: {oldHealth.DisplayName()} → {vm.Health.DisplayName()}", OccurredAt = DateTime.UtcNow });

            if (oldStage != vm.Stage)
                _db.PlantHistory.Add(new PlantHistoryEntry { UserId = UserId(), PlantId = plant.Id, ActivityType = HistoryActivityType.StageChanged, Description = $"Stage changed: {oldStage.DisplayName()} → {vm.Stage.DisplayName()}", OccurredAt = DateTime.UtcNow });

            await _db.SaveChangesAsync();
            TempData["Success"] = $"{plant.Name} updated.";
            return RedirectToAction(nameof(Index));
        }

        // POST /Plants/Delete/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var plant = await _db.Plants.FirstOrDefaultAsync(p => p.Id == id && p.UserId == UserId());
            if (plant == null) return NotFound();

            _db.Plants.Remove(plant);
            await _db.SaveChangesAsync();
            TempData["Success"] = $"{plant.Name} removed from your garden.";
            return RedirectToAction(nameof(Index));
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VEGIBOT.Data;
using VEGIBOT.Models;

namespace VEGIBOT.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public TasksController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        private string UserId() => _userManager.GetUserId(User)!;

        // GET /Tasks
        public async Task<IActionResult> Index(string filter = "all", string? search = null)
        {
            var query = _db.PlantTasks.Include(t => t.Plant).Where(t => t.UserId == UserId());

            query = filter switch
            {
                "pending"   => query.Where(t => !t.IsCompleted),
                "completed" => query.Where(t => t.IsCompleted),
                "overdue"   => query.Where(t => !t.IsCompleted && t.DueDate < DateTime.Today),
                _           => query
            };

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(t => t.Title.Contains(search));

            var vm = new TaskListViewModel
            {
                Filter     = filter,
                Search     = search,
                Tasks      = await query.OrderBy(t => t.IsCompleted).ThenBy(t => t.DueDate).ToListAsync(),
                UserPlants = await _db.Plants.Where(p => p.UserId == UserId()).OrderBy(p => p.Name).ToListAsync()
            };
            return View(vm);
        }

        // GET /Tasks/Create
        public async Task<IActionResult> Create(int? plantId = null)
        {
            ViewBag.Plants = await _db.Plants.Where(p => p.UserId == UserId()).OrderBy(p => p.Name).ToListAsync();
            return View(new TaskFormViewModel { DueDate = DateTime.Today, PlantId = plantId });
        }

        // POST /Tasks/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Plants = await _db.Plants.Where(p => p.UserId == UserId()).OrderBy(p => p.Name).ToListAsync();
                return View(vm);
            }

            _db.PlantTasks.Add(new PlantTask
            {
                UserId      = UserId(),
                PlantId     = vm.PlantId,
                Title       = vm.Title,
                Description = vm.Description,
                Category    = vm.Category,
                Priority    = vm.Priority,
                DueDate     = vm.DueDate,
                IsCompleted = false,
                CreatedAt   = DateTime.UtcNow
            });

            await _db.SaveChangesAsync();
            TempData["Success"] = "Task created.";
            return RedirectToAction(nameof(Index));
        }

        // GET /Tasks/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var task = await _db.PlantTasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == UserId());
            if (task == null) return NotFound();

            ViewBag.Plants = await _db.Plants.Where(p => p.UserId == UserId()).OrderBy(p => p.Name).ToListAsync();
            return View(new TaskFormViewModel
            {
                Id          = task.Id,
                Title       = task.Title,
                Description = task.Description,
                PlantId     = task.PlantId,
                Category    = task.Category,
                Priority    = task.Priority,
                DueDate     = task.DueDate,
                IsCompleted = task.IsCompleted
            });
        }

        // POST /Tasks/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TaskFormViewModel vm)
        {
            if (id != vm.Id) return BadRequest();
            if (!ModelState.IsValid)
            {
                ViewBag.Plants = await _db.Plants.Where(p => p.UserId == UserId()).OrderBy(p => p.Name).ToListAsync();
                return View(vm);
            }

            var task = await _db.PlantTasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == UserId());
            if (task == null) return NotFound();

            task.Title       = vm.Title;
            task.Description = vm.Description;
            task.PlantId     = vm.PlantId;
            task.Category    = vm.Category;
            task.Priority    = vm.Priority;
            task.DueDate     = vm.DueDate;
            await _db.SaveChangesAsync();
            TempData["Success"] = "Task updated.";
            return RedirectToAction(nameof(Index));
        }

        // POST /Tasks/Toggle/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(int id)
        {
            var task = await _db.PlantTasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == UserId());
            if (task == null) return NotFound();

            task.IsCompleted = !task.IsCompleted;
            task.CompletedAt = task.IsCompleted ? DateTime.UtcNow : null;

            if (task.IsCompleted && task.PlantId.HasValue)
                _db.PlantHistory.Add(new PlantHistoryEntry { UserId = UserId(), PlantId = task.PlantId, ActivityType = HistoryActivityType.TaskCompleted, Description = $"Completed task: {task.Title}", OccurredAt = DateTime.UtcNow });

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST /Tasks/Delete/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var task = await _db.PlantTasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == UserId());
            if (task == null) return NotFound();
            _db.PlantTasks.Remove(task);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Task deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}

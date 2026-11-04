using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VEGIBOT.Data;
using VEGIBOT.Models;

namespace VEGIBOT.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public ProfileController(ApplicationDbContext db, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        private string UserId() => _userManager.GetUserId(User)!;


        // GET /Profile
        public async Task<IActionResult> Index()
        {
            var userId = UserId();
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null) return NotFound();

            var vm = new ProfileViewModel
            {
                Email = user.Email ?? string.Empty,
                DisplayName = user.UserName,
                TotalPlants = _db.Plants.Count(p => p.UserId == userId),
                TotalTasks = _db.PlantTasks.Count(t => t.UserId == userId)
            };
            return View(vm);
        }

        // POST /Profile/UpdateDisplayName
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateDisplayName(string displayName)
        {
            var userId = UserId();
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(displayName))
            {
                var result = await _userManager.SetUserNameAsync(user, displayName.Trim());
                if (result.Succeeded)
                {
                    // Refresh the auth cookie so the name in the top bar updates immediately.
                    await _signInManager.RefreshSignInAsync(user);
                    TempData["Success"] = "Display name updated.";
                }
                else
                {
                    TempData["Error"] = string.Join(", ", result.Errors.Select(e => e.Description));
                }
            }
            else
            {
                TempData["Error"] = "Display name is required.";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST /Profile/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = string.Join(" ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .Where(m => !string.IsNullOrWhiteSpace(m)));
                return RedirectToAction(nameof(Index));
            }

            var userId = UserId();
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null) return NotFound();

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (result.Succeeded)
            {
                TempData["Success"] = "Password changed successfully.";
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                TempData["Error"] = $"Password change failed: {errors}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
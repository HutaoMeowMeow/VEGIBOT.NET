using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VEGIBOT.Data;

var builder = WebApplication.CreateBuilder(args);

// ──────────────────────────────────────────────────────────────
// Database & Identity
// ──────────────────────────────────────────────────────────────
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Data Source=vegibot.db"));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount  = false;
    options.Password.RequireDigit          = true;
    options.Password.RequiredLength        = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase      = false;
    options.Lockout.DefaultLockoutTimeSpan  = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers      = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// ──────────────────────────────────────────────────────────────
// Cookie configuration
// ──────────────────────────────────────────────────────────────
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath        = "/Account/Login";
    options.LogoutPath       = "/Account/Logout";
    options.AccessDeniedPath = "/Account/Login";
    options.ExpireTimeSpan   = TimeSpan.FromDays(14);
    options.SlidingExpiration = true;
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// ──────────────────────────────────────────────────────────────
// Startup: migrate database + seed dev account
// ──────────────────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db      = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var env     = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

    // Always apply pending migrations
    await db.Database.MigrateAsync();

    // Seed development test account — DEVELOPMENT ONLY
    // Credentials: test@vegibot.local / VegiBotTest123!
    // Remove or replace with User Secrets before any production deployment.
    if (env.IsDevelopment())
    {
        const string devEmail = "test@vegibot.local";
        const string devPass  = "VegiBotTest123!";   // dev-only; never used in production

        var existing = await userMgr.FindByEmailAsync(devEmail);
        if (existing == null)
        {
            var devUser = new IdentityUser
            {
                UserName       = devEmail,
                Email          = devEmail,
                EmailConfirmed = true
            };
            var result = await userMgr.CreateAsync(devUser, devPass);
            if (!result.Succeeded)
            {
                var errs = string.Join(", ", result.Errors.Select(e => e.Description));
                app.Logger.LogWarning("Dev seed account creation failed: {Errors}", errs);
            }
            else
            {
                app.Logger.LogInformation("Dev seed account created: {Email}", devEmail);
            }
        }
        // Never update the password of an existing account.
    }
}

// ──────────────────────────────────────────────────────────────
// HTTP pipeline
// ──────────────────────────────────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();

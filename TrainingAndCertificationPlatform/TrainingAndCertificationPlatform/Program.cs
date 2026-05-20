using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using TrainingAndCertificationPlatform.Data;
using TrainingAndCertificationPlatform.Services;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<TrainAndCertContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Replace your existing AddAuthentication block with this
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<TrainAndCertContext>();

// Keep your cookie paths here
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddScoped<NotificationService>();
builder.Services.AddAuthorization();

var app = builder.Build();

// Role and user seeding
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider
        .GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles = { "TrainingCoordinator", "Instructor", "Trainee" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    var userManager = scope.ServiceProvider
        .GetRequiredService<UserManager<IdentityUser>>();

    var seedUsers = new[]
    {
        new { Email = "fatima.nasser@gmail.com",  Password = "Admin123",      Role = "TrainingCoordinator" },
        new { Email = "hassan.ali@gmail.com",      Password = "Instructor123", Role = "Instructor"          },
        new { Email = "husain.mohammed@gmail.com", Password = "Instructor123", Role = "Instructor"          },
        new { Email = "sara.ahmed@gmail.com",      Password = "Trainee123",    Role = "Trainee"             },
        new { Email = "ali.jassim@gmail.com",      Password = "Trainee123",    Role = "Trainee"             },
        new { Email = "maryam.khalil@gmail.com",   Password = "Trainee123",    Role = "Trainee"             },
        new { Email = "omar.salman@gmail.com",     Password = "Trainee123",    Role = "Trainee"             },
        new { Email = "zainab.yousuf@gmail.com",   Password = "Trainee123",    Role = "Trainee"             },
    };

    foreach (var u in seedUsers)
    {
        var existing = await userManager.FindByEmailAsync(u.Email);
        if (existing == null)
        {
            var identityUser = new IdentityUser
            {
                UserName = u.Email,
                Email = u.Email,
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(identityUser, u.Password);
            if (result.Succeeded)
                await userManager.AddToRoleAsync(identityUser, u.Role);
        }
    }
}

// Configure the HTTP request pipeline.
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
app.MapRazorPages();
app.Run();
using CineVerse.Application;
using CineVerse.Domain.Entities;
using CineVerse.Infrastructure;
using CineVerse.Infrastructure.Persistence.Contexts;
using CineVerse.MVC;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructure(builder.Configuration.GetConnectionString("DbConn")!);

builder.Services.AddIdentity<AppUser, AppRole>(options =>
{
    options.User.RequireUniqueEmail = true;

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 3;
})
    .AddEntityFrameworkStores<AppDbContext>();


builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseQueryStrings = true;
    options.LowercaseUrls = true;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = new PathString("/Account/Login");
    options.LogoutPath = new PathString("/Account/Logout");
    options.AccessDeniedPath = new PathString("/Account/Login");

    options.Cookie = new()
    {
        Name = "CineVerse.Cookie",
        HttpOnly = true,
        SameSite = SameSiteMode.Strict,
        SecurePolicy = CookieSecurePolicy.Always
    };

    options.SlidingExpiration = false;
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
});

builder.Services.AddAuthentication();

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );

app.MapDefaultControllerRoute();

await DbSeeder.SeedData(app);

app.Run();





// NOT: Bu, çözümün derlenebilir olması için geçici bir başlangıç dosyasıdır.
// 5. adımda; Application/Infrastructure katman kayıtlarını, Identity yapılandırmasını,
// Areas route'unu ve DbSeeder çağrısını buraya ekleyeceğiz (AcademyBlog.MVC/Program.cs ile birebir aynı desende).

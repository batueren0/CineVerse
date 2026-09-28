var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.MapDefaultControllerRoute();

app.Run();

// NOT: Bu, çözümün derlenebilir olması için geçici bir başlangıç dosyasıdır.
// 5. adımda; Application/Infrastructure katman kayıtlarını, Identity yapılandırmasını,
// Areas route'unu ve DbSeeder çağrısını buraya ekleyeceğiz (AcademyBlog.MVC/Program.cs ile birebir aynı desende).

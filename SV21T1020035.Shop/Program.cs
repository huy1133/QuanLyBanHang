using Microsoft.AspNetCore.Authentication.Cookies;
using SV21T1020035.Shop.AppCodes;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

builder.Services.AddControllersWithViews()
    .AddMvcOptions(option =>
    {
        option.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
    });
// Cấu hình Authentication và Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option =>
    {
        option.Cookie.Name = "AuthenticationCookie";
        option.LoginPath = "/Account/Login";
        option.AccessDeniedPath = "/Account/AccessDenied";
        option.ExpireTimeSpan = TimeSpan.FromDays(360);
    });

// Thêm Session
builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromMinutes(60);
    option.Cookie.HttpOnly = true;
    option.Cookie.IsEssential = true;
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();
app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

ApplicationContext.Configure(
    context: app.Services.GetRequiredService<IHttpContextAccessor>(),
    enviroment: app.Services.GetRequiredService<IWebHostEnvironment>()
    );
//khởi tạo cấu hình cho BusinessLayer 
string connectionString = builder.Configuration.GetConnectionString("LiteCommerceDB") ?? "";
SV21T1020035.BusinessLayers.Configuration.Initialize(connectionString);
app.Run();

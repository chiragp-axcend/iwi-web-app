using InternationalWaterWebApp.Library.DatabaseConnection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Configuration;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);
DatabaseConnection.connectionString = builder.Configuration.GetConnectionString("WaterConnection");
DefaultValueFromWebConfig.SMTPSERVER = builder.Configuration.GetValue<string>("Keys:SMTPSERVER");
DefaultValueFromWebConfig.SMTPUsername = builder.Configuration.GetValue<string>("Keys:SMTPUsername");
DefaultValueFromWebConfig.SMTPPassword = builder.Configuration.GetValue<string>("Keys:SMTPPassword");
DefaultValueFromWebConfig.SMTPSSL = builder.Configuration.GetValue<string>("Keys:SMTPSSL");
DefaultValueFromWebConfig.SMTPPort = builder.Configuration.GetValue<string>("Keys:SMTPPort");
DefaultValueFromWebConfig.EmailFrom = builder.Configuration.GetValue<string>("Keys:EmailFrom");
DefaultValueFromWebConfig.ExcelConString = builder.Configuration.GetConnectionString("ExcelConString");
DefaultValueFromWebConfig.Rusle2APIURL = builder.Configuration.GetSection("Keys:ApiUrls").GetSection("Rusle2APIURL").Value;
RootDirectoryPath.SiteURL = builder.Configuration.GetValue<string>("Keys:SiteURL");

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddMemoryCache();

//TimeSpan CookieSessionExpiry = TimeSpan.FromMinutes(20);
TimeSpan CookieSessionExpiry = TimeSpan.FromHours(8);

builder.Services.AddSession(options =>
{
    options.IdleTimeout = CookieSessionExpiry;
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Common/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
RootDirectoryPath.ContentRootPath = app.Environment.ContentRootPath;
RootDirectoryPath.WebRootPath = app.Environment.WebRootPath;

app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=WebApp}/{action=Login}");
app.Run();

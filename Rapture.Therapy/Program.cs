using Eadent.Identity.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using NLog.Web;
using Rapture.Therapy.Configuration;
using Rapture.Therapy.DataAccess.RaptureTherapy.Databases;
using Rapture.Therapy.DataAccess.RaptureTherapy.Repositories;
using Rapture.Therapy.Sessions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Configuration.AddJsonFile("Confidential/Eadent.Identity.settings.json", optional: false, reloadOnChange: false);
builder.Configuration.AddJsonFile("Confidential/Rapture.Therapy.settings.json", optional: false, reloadOnChange: false);

// NLog: Setup NLog for Dependency I    njection.
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
builder.Host.UseNLog();

var services = builder.Services;

services.AddDistributedMemoryCache();

services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

services.AddHttpContextAccessor();

Eadent.Identity.Startup.ConfigureServices(services, builder.Configuration.GetSection(EadentIdentitySettings.SectionName).Get<EadentIdentitySettings>());

var databaseSettings = builder.Configuration.GetSection(RaptureTherapySettings.SectionName).Get<RaptureTherapySettings>().Database;

string connectionString = $"Server={databaseSettings.DatabaseServer};Database={databaseSettings.DatabaseName};Application Name={databaseSettings.ApplicationName};User Id={databaseSettings.UserName};Password={databaseSettings.Password};";

services.AddDbContext<RaptureTherapyDatabase>(options => options.UseSqlServer(connectionString));

services.AddScoped<IRaptureTherapyDatabase, RaptureTherapyDatabase>();

services.AddTransient<IRaptureTherapyDatabaseVersionsRepository, RaptureTherapyDatabaseVersionsRepository>();

services.AddTransient<IUserSession, UserSession>();

services.AddRazorPages();
services.AddDirectoryBrowser();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseDirectoryBrowser(new DirectoryBrowserOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.WebRootPath, "Preliminary")),
    RequestPath = "/Preliminary"
});

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapRazorPages();

app.Run();

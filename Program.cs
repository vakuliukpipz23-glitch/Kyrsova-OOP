using Kyrsova_OOP.Repositories;
using Kyrsova_OOP.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IHabitRepository, HabitRepository>();
builder.Services.AddSingleton<HabitManager>();
builder.Services.AddSingleton<StatisticsService>();

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Habit}/{action=Index}/{id?}");

app.Run();
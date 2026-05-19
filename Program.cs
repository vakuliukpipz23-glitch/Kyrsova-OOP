using Kyrsova_OOP.Repositories;
using Kyrsova_OOP.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets(typeof(Program).Assembly, optional: true);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=habits.db;Pooling=False";

var db = new DatabaseContext(connectionString);
db.Initialize();

builder.Services.AddControllersWithViews();

builder.Services.AddSingleton(db);

builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("Smtp"));

builder.Services.AddSingleton<IHabitRepository, HabitRepository>();
builder.Services.AddSingleton<HabitManager>();
builder.Services.AddSingleton<StatisticsService>();
builder.Services.AddSingleton<IEmailSender, SmtpEmailSender>();
builder.Services.AddSingleton<HabitReminderService>();
builder.Services.AddHostedService<DailyReminderHostedService>();

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Habit}/{action=Index}/{id?}");

app.Run();
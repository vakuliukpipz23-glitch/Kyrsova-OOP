using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text;
using Microsoft.Data.Sqlite;
using Kyrsova_OOP.Services;

namespace Kyrsova_OOP.Controllers
{
    public class HabitController : Controller
    {
        private readonly HabitManager manager;
        private readonly StatisticsService statistics;
        private readonly IEmailSender emailSender;

        public HabitController(HabitManager manager, StatisticsService statistics, IEmailSender emailSender)
        {
            this.manager = manager;
            this.statistics = statistics;
            this.emailSender = emailSender;
        }

        public IActionResult Index()
        {
            var habits = manager.GetAll();

            return View(habits);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                ViewBag.Error = "Назва не може бути порожньою.";
                return View();
            }

            try
            {
                manager.Create(name.Trim());
            }
            catch (SqliteException)
            {
                ViewBag.Error = "База даних тимчасово зайнята. Спробуйте ще раз.";
                return View();
            }
            catch (InvalidOperationException)
            {
                ViewBag.Error = "База даних тимчасово зайнята. Спробуйте ще раз.";
                return View();
            }

            return RedirectToAction("Index");
        }

        public IActionResult Create(string name)
        {
            manager.Create(name);

            return RedirectToAction("Index");
        }

        public IActionResult Complete(int id)
        {
            manager.Complete(id);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                manager.Delete(id);
            }
            catch (SqliteException)
            {
                TempData["DeleteError"] = "База даних тимчасово зайнята. Спробуйте ще раз.";
            }
            catch (InvalidOperationException)
            {
                TempData["DeleteError"] = "База даних тимчасово зайнята. Спробуйте ще раз.";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var habit = manager.Get(id);
            if (habit is null)
            {
                return NotFound();
            }

            return View(habit);
        }

        [HttpPost]
        public IActionResult Edit(int id, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                var habit = manager.Get(id);
                if (habit is null)
                {
                    return NotFound();
                }

                ViewBag.Error = "Назва не може бути порожньою.";
                return View(habit);
            }

            try
            {
                manager.Update(id, name.Trim());
            }
            catch (SqliteException)
            {
                var habit = manager.Get(id);
                if (habit is null)
                {
                    return NotFound();
                }

                ViewBag.Error = "База даних тимчасово зайнята. Спробуйте ще раз.";
                return View(habit);
            }
            catch (InvalidOperationException)
            {
                var habit = manager.Get(id);
                if (habit is null)
                {
                    return NotFound();
                }

                ViewBag.Error = "База даних тимчасово зайнята. Спробуйте ще раз.";
                return View(habit);
            }

            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            var habit = manager.Get(id);
            if (habit is null)
            {
                return NotFound();
            }

            ViewBag.CurrentStreak = statistics.GetCurrentStreak(habit);
            ViewBag.LongestStreak = statistics.GetLongestStreak(habit);
            ViewBag.Total = habit.GetTotalCompletions();

            return View(habit);
        }

        public IActionResult History(int id)
        {
            var habit = manager.Get(id);
            if (habit is null)
            {
                return NotFound();
            }

            return View(habit);
        }

        public IActionResult Statistics()
        {
            var habits = manager.GetAll().ToList();

            ViewBag.TotalHabits = habits.Count;
            ViewBag.TotalCompletions = habits.Sum(h => h.GetTotalCompletions());
            ViewBag.BestStreak = habits.Count == 0 ? 0 : habits.Max(h => statistics.GetLongestStreak(h));

            return View(habits);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendTestEmail()
        {
            try
            {
                var incomplete = manager.GetAll()
                    .Where(h => !h.IsCompletedToday())
                    .ToList();

                if (incomplete.Count == 0)
                {
                    TempData["TestEmailStatus"] = "Всі звички виконані сьогодні. Нагадування не відправлено.";
                    TempData.Remove("TestEmailPreview");
                    return RedirectToAction("Index");
                }

                var builder = new StringBuilder();
                builder.AppendLine("Ти не виконав деякі звички сьогодні:");
                foreach (var habit in incomplete)
                {
                    builder.AppendLine($"- {habit.Name}");
                }
                builder.AppendLine();
                builder.AppendLine("Будь ласка, виконай їх сьогодні.");

                TempData["TestEmailPreview"] = builder.ToString().TrimEnd();

                await emailSender.SendAsync(
                    "Нагадування про звички",
                    builder.ToString().TrimEnd(),
                    HttpContext.RequestAborted);
                TempData["TestEmailStatus"] = "Нагадування відправлено. Перевірте пошту або Spam.";
            }
            catch
            {
                TempData["TestEmailStatus"] = "Не вдалося відправити тестовий лист.";
            }

            return RedirectToAction("Index");
        }
    }
}
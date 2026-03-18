using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.Data.Sqlite;
using Kyrsova_OOP.Services;

namespace Kyrsova_OOP.Controllers
{
    public class HabitController : Controller
    {
        private readonly HabitManager manager;
        private readonly StatisticsService statistics;

        public HabitController(HabitManager manager, StatisticsService statistics)
        {
            this.manager = manager;
            this.statistics = statistics;
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
    }
}
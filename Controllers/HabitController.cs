using Microsoft.AspNetCore.Mvc;
using Kyrsova_OOP.Services;
using Kyrsova_OOP.Models;
using System.Linq;

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
            var habits = manager.GetHabits();
            return View(habits);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(string title)
        {
            manager.AddHabit(title);
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var habit = manager.GetHabit(id);

            if (habit == null)
                return NotFound();

            return View(habit);
        }

        [HttpPost]
        public IActionResult Edit(int id, string title)
        {
            manager.EditHabit(id, title);
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            manager.DeleteHabit(id);
            return RedirectToAction("Index");
        }

        public IActionResult Complete(int id)
        {
            var habit = manager.GetHabit(id);

            if (habit == null)
                return NotFound();

            manager.MarkHabitDone(id);

            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            var habit = manager.GetHabit(id);

            if (habit == null)
                return NotFound();

            ViewBag.CurrentStreak = statistics.CalculateCurrentStreak(habit);
            ViewBag.LongestStreak = statistics.CalculateLongestStreak(habit);
            ViewBag.Total = statistics.CalculateTotalCompletions(habit);

            return View(habit);
        }

        public IActionResult History(int id)
        {
            var habit = manager.GetHabit(id);

            if (habit == null)
                return NotFound();

            return View(habit);
        }

        public IActionResult Statistics()
       {
         var habits = manager.GetHabits();

         int totalHabits = habits.Count;
         int totalCompletions = habits.Sum(h => h.GetTotalCompletions());
         int bestStreak = habits.Count > 0 ? habits.Max(h => h.GetLongestStreak()) : 0;

         ViewBag.TotalHabits = totalHabits;
         ViewBag.TotalCompletions = totalCompletions;
         ViewBag.BestStreak = bestStreak;

         return View(habits);
       }
    }
}
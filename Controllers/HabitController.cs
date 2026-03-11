using Microsoft.AspNetCore.Mvc;
using Kyrsova_OOP.Repositories;
using Kyrsova_OOP.Services;
using System.Linq;

namespace Kyrsova_OOP.Controllers
{
    public class HabitController : Controller
    {
        private HabitService service;

        public HabitController()
        {
            var repository = new HabitRepository();
            service = new HabitService(repository);

            if (service.GetAllHabits().Count == 0)
            {
                service.CreateHabit("Read book", 1);
                service.CreateHabit("Exercise", 2);
            }
        }

        public IActionResult Index()
        {
            var habits = service.GetAllHabits();

            ViewBag.TotalHabits = habits.Count;
            ViewBag.TotalCompletions = habits.Sum(h => h.GetTotalCompletions());
            ViewBag.BestStreak = habits.Count > 0 ? habits.Max(h => h.GetLongestStreak()) : 0;

            return View(habits);
        }

        public IActionResult Complete(int id)
        {
            service.CompleteHabit(id);
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            service.DeleteHabit(id);
            return RedirectToAction("Index");
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(string title, int priority)
        {
            service.CreateHabit(title, priority);
            return RedirectToAction("Index");
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Kyrsova_OOP.Services;
using Kyrsova_OOP.Repositories;
using Kyrsova_OOP.Models;

namespace Kyrsova_OOP.Controllers
{
    public class HabitController : Controller
    {
        private readonly HabitService service;

        public HabitController()
        {
            var repository = new HabitRepository();
            service = new HabitService(repository);

            // тестові звички щоб сторінка не була пустою
            if (service.GetAllHabits().Count == 0)
            {
                service.CreateHabit("Read book", 1);
                service.CreateHabit("Exercise", 2);
                service.CreateHabit("Study programming", 3);
            }
        }

        public IActionResult Index()
        {
            var habits = service.GetAllHabits();
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
    }
}
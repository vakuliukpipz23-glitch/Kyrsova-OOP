using Microsoft.AspNetCore.Mvc;
using Kyrsova_OOP.Repositories;
using Kyrsova_OOP.Services;

namespace Kyrsova_OOP.Controllers
{
    public class HabitController : Controller
    {
        private HabitService service;
        private HabitRepository repository;

        public HabitController()
        {
            repository = new HabitRepository();
            service = new HabitService(repository);

            if (repository.GetAll().Count == 0)
            {
                service.CreateHabit("Read Book", 1);
                service.CreateHabit("Exercise", 2);
            }
        }

        public IActionResult Index()
        {
            return View(service.GetAll());
        }

        public IActionResult Complete(int id)
        {
            service.MarkCompleted(id);
            return RedirectToAction("Index");
        }
    }
}
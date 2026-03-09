using System.Linq;
using Kyrsova_OOP.Models;
using Kyrsova_OOP.Repositories;

namespace Kyrsova_OOP.Services
{
    public class HabitService
    {
        private IHabitRepository repository;

        public HabitService(IHabitRepository repository)
        {
            this.repository = repository;
        }

        public void CreateHabit(string title, int priority)
        {
            var habit = new Habit
            {
                Id = repository.GetAll().Count + 1,
                Title = title,
                Priority = priority
            };

            repository.Add(habit);
        }

        public void MarkCompleted(int id)
        {
            var habit = repository.GetAll().FirstOrDefault(h => h.Id == id);

            if (habit != null)
                habit.Complete();
        }

        public List<Habit> GetAll()
        {
            return repository.GetAll();
        }
    }
}
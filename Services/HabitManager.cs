using Kyrsova_OOP.Models;
using Kyrsova_OOP.Repositories;

namespace Kyrsova_OOP.Services
{
    public class HabitManager
    {
        private readonly IHabitRepository repository;

        public HabitManager(IHabitRepository repository)
        {
            this.repository = repository;
        }

        public List<Habit> GetHabits()
        {
            return repository.GetAll();
        }

        public Habit? GetHabit(int id)
        {
            return repository.GetById(id);
        }

        public void AddHabit(string title)
        {
            var habit = new Habit
            {
                Title = title,
                CreatedDate = DateTime.Today
            };

            repository.Add(habit);
        }

        public void EditHabit(int id, string title)
        {
            var habit = repository.GetById(id);

            if (habit != null)
            {
                habit.Title = title;
            }
        }

        public void DeleteHabit(int id)
        {
            repository.Delete(id);
        }

        public void MarkHabitDone(int id)
        {
            var habit = repository.GetById(id);

            habit?.AddCompletion();
        }
    }
}
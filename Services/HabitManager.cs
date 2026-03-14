using System.Collections.Generic;
using Kyrsova_OOP.Repositories;
using Kyrsova_OOP.Strategies;
using Kyrsova_OOP.Models;

namespace Kyrsova_OOP.Services
{
    public class HabitManager
    {
        private readonly IHabitRepository repository;
        private readonly INotificationStrategy notification;

        public HabitManager(IHabitRepository repo, INotificationStrategy strategy)
        {
            repository = repo;
            notification = strategy;
        }

        // Backwards-compatible constructor used by tests
        public HabitManager(IHabitRepository repo)
            : this(repo, new Kyrsova_OOP.Strategies.NullNotification())
        {
        }

        public List<Habit> GetAll()
        {
            return repository.GetAll();
        }

        public Habit? Get(int id)
        {
            return repository.GetById(id);
        }

        public void Create(string name)
        {
            repository.Add(new Habit(name));
        }

        public void Delete(int id)
        {
            repository.Delete(id);
        }

        public void MarkCompleted(int id)
        {
            var habit = repository.GetById(id);

            if (habit == null) return;

            habit.AddCompletion();

            notification.Send($"Habit '{habit.Name}' completed today");

            repository.Update(habit);
        }

        // Backwards-compatible API used by controllers/views
        public List<Habit> GetHabits() => GetAll();

        public void AddHabit(string title) => Create(title);

        public Habit? GetHabit(int id) => Get(id);

        public void EditHabit(int id, string title)
        {
            var habit = repository.GetById(id);
            if (habit == null) return;
            habit.Name = title;
            repository.Update(habit);
        }

        public void DeleteHabit(int id) => Delete(id);

        public void MarkHabitDone(int id) => MarkCompleted(id);
    }
}
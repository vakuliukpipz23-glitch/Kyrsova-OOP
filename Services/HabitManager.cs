using System.Collections.Generic;
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

        public IEnumerable<Habit> GetAll()
        {
            return repository.GetAll();
        }

        public Habit? Get(int id)
        {
            return repository.GetById(id);
        }

        public void Create(string name)
        {
            var habit = new Habit
            {
                Name = name
            };

            repository.Add(habit);
        }

        public void Update(int id, string name)
        {
            var habit = repository.GetById(id);
            if (habit is null)
            {
                return;
            }

            habit.Name = name;
            repository.Update(habit);
        }

        public void Delete(int id)
        {
            repository.Delete(id);
        }

        public void Complete(int id)
        {
            var habit = repository.GetById(id);

            if (habit is null)
            {
                return;
            }

            habit.AddCompletion();
            repository.Update(habit);
        }
    }
}
using System.Collections.Generic;
using Kyrsova_OOP.Models;
using Kyrsova_OOP.Repositories;

namespace Kyrsova_OOP.Services
{
    public class HabitService
    {
        private readonly IHabitRepository repository;

        public HabitService(IHabitRepository repository)
        {
            this.repository = repository;
        }

        public void CreateHabit(string title, int priority)
        {
            int id = repository.GetAll().Count + 1;

            Habit habit = new Habit
            {
                Id = id,
                Title = title,
                Priority = priority
            };

            repository.Add(habit);
        }

        public void CompleteHabit(int id)
        {
            var habit = repository.GetById(id);

            if (habit != null)
            {
                habit.Complete();
            }
        }

        public List<Habit> GetAllHabits()
        {
            return repository.GetAll();
        }

        public Habit? GetHabit(int id)
        {
            return repository.GetById(id);
        }

        public void DeleteHabit(int id)
        {
            repository.Remove(id);
        }
    }
}
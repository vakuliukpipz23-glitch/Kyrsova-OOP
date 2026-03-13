using Kyrsova_OOP.Models;

namespace Kyrsova_OOP.Repositories
{
    public class HabitRepository : IHabitRepository
    {
        private readonly List<Habit> habits = new();

        private int nextId = 1;

        public List<Habit> GetAll()
        {
            return habits;
        }

        public Habit? GetById(int id)
        {
            return habits.FirstOrDefault(h => h.Id == id);
        }

        public void Add(Habit habit)
        {
            habit.Id = nextId++;
            habits.Add(habit);
        }

        public void Update(Habit habit)
        {
        }

        public void Delete(int id)
        {
            var habit = GetById(id);

            if (habit != null)
            {
                habits.Remove(habit);
            }
        }
    }
}
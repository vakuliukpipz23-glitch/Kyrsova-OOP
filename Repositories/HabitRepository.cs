using System.Collections.Generic;
using System.Linq;
using Kyrsova_OOP.Models;

namespace Kyrsova_OOP.Repositories
{
    public class HabitRepository : IHabitRepository
    {
        private static List<Habit> habits = new List<Habit>();

        public void Add(Habit habit)
        {
            habits.Add(habit);
        }

        public void Remove(int id)
        {
            habits.RemoveAll(h => h.Id == id);
        }

        public List<Habit> GetAll()
        {
            return habits;
        }
    }
}
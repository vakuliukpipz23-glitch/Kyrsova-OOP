using Kyrsova_OOP.Models;
using System.Text.Json;

namespace Kyrsova_OOP.Repositories
{
    public class HabitRepository : IHabitRepository
    {
        private List<Habit> habits = new();

        private int nextId = 1;

        private readonly string filePath = "habits.json";

        public HabitRepository()
        {
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);

                habits = JsonSerializer.Deserialize<List<Habit>>(json) ?? new List<Habit>();

                if (habits.Any())
                {
                    nextId = habits.Max(h => h.Id) + 1;
                }
            }
        }

        private void Save()
        {
            var json = JsonSerializer.Serialize(habits);
            File.WriteAllText(filePath, json);
        }

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
            Save();
        }

        public void Update(Habit habit)
        {
            Save();
        }

        public void Delete(int id)
        {
            var habit = GetById(id);

            if (habit != null)
            {
                habits.Remove(habit);
                Save();
            }
        }
    }
}
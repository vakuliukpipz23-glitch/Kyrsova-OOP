using System.Linq;
using Kyrsova_OOP.Models;

namespace Kyrsova_OOP.Services
{
    public class StatisticsService
    {
        public int GetCurrentStreak(Habit habit)
        {
            return habit.GetCurrentStreak();
        }

        public int GetLongestStreak(Habit habit)
        {
            return habit.GetLongestStreak();
        }

        public int GetTotalCompletions(Habit habit)
        {
            return habit.Records.Count;
        }

        // Backwards-compatible method names used by controllers
        public int CalculateCurrentStreak(Habit habit) => GetCurrentStreak(habit);

        public int CalculateLongestStreak(Habit habit) => GetLongestStreak(habit);

        public int CalculateTotalCompletions(Habit habit) => GetTotalCompletions(habit);
    }
}
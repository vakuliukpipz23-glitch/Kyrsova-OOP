using Kyrsova_OOP.Models;

namespace Kyrsova_OOP.Services
{
    public class StatisticsService
    {
        public int CalculateCurrentStreak(Habit habit)
        {
            return habit.GetCurrentStreak();
        }

        public int CalculateLongestStreak(Habit habit)
        {
            return habit.GetLongestStreak();
        }

        public int CalculateTotalCompletions(Habit habit)
        {
            return habit.GetTotalCompletions();
        }
    }
}
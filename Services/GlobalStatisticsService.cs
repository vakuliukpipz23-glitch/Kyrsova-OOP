using System.Collections.Generic;
using System.Linq;
using Kyrsova_OOP.Models;

namespace Kyrsova_OOP.Services
{
    public class GlobalStatisticsService
    {
        public int GetTotalHabits(List<Habit> habits)
        {
            return habits.Count;
        }

        public int GetTotalCompletions(List<Habit> habits)
        {
            return habits.Sum(h => h.Records.Count);
        }

        public int GetBestStreak(List<Habit> habits)
        {
            if (habits.Count == 0)
                return 0;

            return habits.Max(h => h.GetLongestStreak());
        }
    }
}
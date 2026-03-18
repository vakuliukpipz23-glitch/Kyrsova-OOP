using System;
using System.Linq;
using Kyrsova_OOP.Models;

namespace Kyrsova_OOP.Services
{
    public class StatisticsService
    {
        public int GetCurrentStreak(Habit habit)
        {
            var ordered = habit.Records
                .OrderByDescending(r => r.Date)
                .ToList();

            int streak = 0;
            DateTime date = DateTime.Today;

            foreach (var record in ordered)
            {
                if (record.Date.Date == date.Date)
                {
                    streak++;
                    date = date.AddDays(-1);
                }
                else
                {
                    break;
                }
            }

            return streak;
        }

        public int GetLongestStreak(Habit habit)
        {
            var dates = habit.Records
                .Select(r => r.Date.Date)
                .Distinct()
                .OrderBy(d => d)
                .ToList();

            if (dates.Count == 0) return 0;

            int longest = 1;
            int current = 1;

            for (int i = 1; i < dates.Count; i++)
            {
                if (dates[i] == dates[i - 1].AddDays(1))
                {
                    current++;
                    longest = Math.Max(longest, current);
                }
                else
                {
                    current = 1;
                }
            }

            return longest;
        }

        public double GetCompletionRate(Habit habit)
        {
            var start = habit.CreatedDate.Date;
            var end = DateTime.Today;
            var days = (end - start).Days + 1;

            if (days <= 0)
                return 0;

            var completedDays = habit.Records
                .Select(r => r.Date.Date)
                .Where(d => d >= start && d <= end)
                .Distinct()
                .Count();

            var rate = (double)completedDays / days * 100;
            return Math.Clamp(rate, 0, 100);
        }
    }
}
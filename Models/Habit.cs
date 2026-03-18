using System;
using System.Collections.Generic;
using System.Linq;

namespace Kyrsova_OOP.Models
{
    public class Habit
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public List<HabitRecord> Records { get; set; } = new List<HabitRecord>();


        public void AddCompletion()
        {
            if (!IsCompletedToday())
            {
                Records.Add(new HabitRecord
                {
                    Date = DateTime.Today
                });
            }
        }

        public bool IsCompletedToday()
        {
            return Records.Any(r => r.Date.Date == DateTime.Today);
        }

        public int GetTotalCompletions()
        {
            return Records.Count;
        }

        public int GetLongestStreak()
        {
            if (!Records.Any()) return 0;

            var sortedDates = Records
                .Select(r => r.Date.Date)
                .OrderBy(d => d)
                .Distinct()
                .ToList();

            int maxStreak = 1;
            int currentStreak = 1;

            for (int i = 1; i < sortedDates.Count; i++)
            {
                if (sortedDates[i] == sortedDates[i - 1].AddDays(1))
                {
                    currentStreak++;
                    maxStreak = Math.Max(maxStreak, currentStreak);
                }
                else
                {
                    currentStreak = 1;
                }
            }

            return maxStreak;
        }

        public int GetCurrentStreak()
        {
            if (!Records.Any()) return 0;

            var ordered = Records
                .OrderByDescending(r => r.Date)
                .ToList();

            int streak = 0;
            DateTime date = DateTime.Today;

            foreach (var record in ordered)
            {
                if (record.Date.Date == date)
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

        public double GetCompletionRate()
        {
            var start = CreatedDate.Date;
            var end = DateTime.Today;
            var days = (end - start).Days + 1;
            if (days <= 0) return 0;

            var completedDays = Records
                .Select(r => r.Date.Date)
                .Where(d => d >= start && d <= end)
                .Distinct()
                .Count();

            var rate = (double)completedDays / days * 100;
            return Math.Clamp(rate, 0, 100);
        }
    }
}
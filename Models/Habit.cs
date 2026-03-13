using System;
using System.Collections.Generic;
using System.Linq;

namespace Kyrsova_OOP.Models
{
    public class Habit
    {
        public int Id { get; set; }

        public string Title { get; set;} = "";

        public DateTime CreatedDate { get; set; }

        public List<HabitRecord> Records { get; set; } = new();

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

        public int GetCurrentStreak()
        {
            int streak = 0;
            DateTime day = DateTime.Today;

            while (Records.Any(r => r.Date.Date == day))
            {
                streak++;
                day = day.AddDays(-1);
            }

            return streak;
        }

        public int GetLongestStreak()
        {
            var dates = Records
                .Select(r => r.Date.Date)
                .OrderBy(d => d)
                .ToList();

            int longest = 0;
            int current = 1;

            for (int i = 1; i < dates.Count; i++)
            {
                if (dates[i] == dates[i - 1].AddDays(1))
                {
                    current++;
                }
                else
                {
                    longest = Math.Max(longest, current);
                    current = 1;
                }
            }

            return Math.Max(longest, current);
        }

        public int GetTotalCompletions()
        {
            return Records.Count;
        }

        public double GetCompletionRate()
        {
            int days = (DateTime.Today - CreatedDate).Days + 1;

            if (days <= 0)
                return 0;

            return (double)Records.Count / days * 100;
        }
    }
}
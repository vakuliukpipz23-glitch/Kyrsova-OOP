using System;
using System.Collections.Generic;
using System.Linq;

namespace Kyrsova_OOP.Models
{
    public class Habit
    {
        public int Id { get; set; }

        public string Title { get; set; } = "";

        public int Priority { get; set; }

        public List<CompletionRecord> Records { get; set; } = new();

        public void Complete()
        {
            Records.Add(new CompletionRecord(DateTime.Now, true));
        }

        public bool IsCompletedToday()
        {
            return Records.Any(r => r.Date.Date == DateTime.Today && r.Completed);
        }

        public int GetCurrentStreak()
        {
            if (Records.Count == 0)
                return 0;

            int streak = 0;

            var dates = Records
                .Where(r => r.Completed)
                .Select(r => r.Date.Date)
                .Distinct()
                .OrderByDescending(d => d)
                .ToList();

            DateTime expectedDate = DateTime.Today;

            foreach (var date in dates)
            {
                if (date == expectedDate)
                {
                    streak++;
                    expectedDate = expectedDate.AddDays(-1);
                }
                else
                {
                    break;
                }
            }

            return streak;
        }

        public int GetLongestStreak()
        {
            if (Records.Count == 0)
                return 0;

            var dates = Records
                .Where(r => r.Completed)
                .Select(r => r.Date.Date)
                .Distinct()
                .OrderBy(d => d)
                .ToList();

            int longest = 0;
            int current = 0;

            for (int i = 0; i < dates.Count; i++)
            {
                if (i == 0 || dates[i] == dates[i - 1].AddDays(1))
                {
                    current++;
                }
                else
                {
                    current = 1;
                }

                if (current > longest)
                    longest = current;
            }

            return longest;
        }

        public int GetTotalCompletions()
        {
            return Records.Count(r => r.Completed);
        }

        public double GetCompletionRate()
        {
            if (Records.Count == 0)
                return 0;

            return (double)GetTotalCompletions() / Records.Count * 100;
        }
    }
}
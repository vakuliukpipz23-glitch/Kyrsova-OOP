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

        public int GetCurrentStreak()
        {
            int streak = 0;

            foreach (var record in Records.OrderByDescending(r => r.Date))
            {
                if (record.Completed)
                    streak++;
                else
                    break;
            }

            return streak;
        }

        public int GetTotalCompletions()
        {
            return Records.Count(r => r.Completed);
        }

        public int GetLongestStreak()
        {
            int longest = 0;
            int current = 0;

            foreach (var record in Records.OrderBy(r => r.Date))
            {
                if (record.Completed)
                {
                    current++;
                    if (current > longest)
                        longest = current;
                }
                else
                {
                    current = 0;
                }
            }

            return longest;
        }

        public double GetCompletionRate()
        {
            if (Records.Count == 0)
                return 0;

            return (double)GetTotalCompletions() / Records.Count * 100;
        }
    }
}
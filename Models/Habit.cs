using System;
using System.Collections.Generic;
using System.Linq;
using Kyrsova_OOP.Observers;

namespace Kyrsova_OOP.Models
{
    public class Habit : IEquatable<Habit>
    {
        public Habit()
        {
            Name = string.Empty;
            CreatedDate = DateTime.Today;
            Records = new List<HabitRecord>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        
        public string Title
        {
            get => Name;
            set => Name = value;
        }

        public List<HabitRecord> Records { get; set; } = new List<HabitRecord>();

        private List<IObserver> observers = new List<IObserver>();

        public Habit(string name)
        {
            Name = name;
            CreatedDate = DateTime.Today;
        }

        public void AddObserver(IObserver observer)
        {
            observers.Add(observer);
        }

        public void RemoveObserver(IObserver observer)
        {
            observers.Remove(observer);
        }

        private void NotifyObservers()
        {
            foreach (var observer in observers)
            {
                observer.Update(this);
            }
        }

        public void AddCompletion()
        {
            if (!Records.Any(r => r.Date.Date == DateTime.Today))
            {
                Records.Add(new HabitRecord { Date = DateTime.Today });

                NotifyObservers();
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
            if (Records.Count == 0) return 0;

            var dates = Records
                .Select(r => r.Date.Date)
                .Distinct()
                .OrderBy(d => d)
                .ToList();

            int longest = 1;
            int current = 1;

            for (int i = 1; i < dates.Count; i++)
            {
                if ((dates[i] - dates[i - 1]).Days == 1)
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

        public int GetTotalCompletions()
        {
            return Records?.Count ?? 0;
        }

        public double GetCompletionRate()
        {
            var total = GetTotalCompletions();
            var days = (DateTime.Today - CreatedDate).Days + 1;
            if (days <= 0) return 0;

            var rate = (double)total / days * 100.0;
            return Math.Min(100, rate); // Ensure values never exceed 100%
        }

        public bool Equals(Habit? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id != 0 && other.Id != 0 ? Id == other.Id : string.Equals(Name, other.Name, StringComparison.Ordinal);
        }

        public override bool Equals(object? obj) => Equals(obj as Habit);

        public override int GetHashCode()
        {
            return Id != 0 ? Id.GetHashCode() : (Name?.GetHashCode() ?? 0);
        }
    }
}
using System;
using Kyrsova_OOP.Models;
using Kyrsova_OOP.Services;

namespace Kyrsova_OOP.Tests
{
    public class StatisticsServiceTests
    {
        [Fact]
        public void GetCurrentStreak_ReturnsZeroWhenNoRecords()
        {
            var service = new StatisticsService();
            var habit = new Habit();

            Assert.Equal(0, service.GetCurrentStreak(habit));
        }

        [Fact]
        public void GetCurrentStreak_ReturnsCorrectStreak()
        {
            var service = new StatisticsService();
            var habit = new Habit();
            habit.Records.Add(new HabitRecord { Date = DateTime.Today });
            habit.Records.Add(new HabitRecord { Date = DateTime.Today.AddDays(-1) });

            Assert.Equal(2, service.GetCurrentStreak(habit));
        }

        [Fact]
        public void GetLongestStreak_ReturnsCorrectLongestSequence()
        {
            var service = new StatisticsService();
            var habit = new Habit();
            habit.Records.Add(new HabitRecord { Date = DateTime.Today.AddDays(-5) });
            habit.Records.Add(new HabitRecord { Date = DateTime.Today.AddDays(-4) });
            habit.Records.Add(new HabitRecord { Date = DateTime.Today.AddDays(-2) });
            habit.Records.Add(new HabitRecord { Date = DateTime.Today.AddDays(-1) });

            Assert.Equal(2, service.GetLongestStreak(habit));
        }
    }
}

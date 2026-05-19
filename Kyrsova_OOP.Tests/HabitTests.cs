using System;
using System.Linq;
using Kyrsova_OOP.Models;

namespace Kyrsova_OOP.Tests
{
    public class HabitTests
    {
        [Fact]
        public void AddCompletion_AddsRecordForToday()
        {
            var habit = new Habit();

            habit.AddCompletion();

            Assert.Single(habit.Records);
            Assert.Equal(DateTime.Today, habit.Records.Single().Date.Date);
        }

        [Fact]
        public void AddCompletion_DoesNotAddDuplicateForSameDay()
        {
            var habit = new Habit();

            habit.AddCompletion();
            habit.AddCompletion();

            Assert.Single(habit.Records);
        }

        [Fact]
        public void IsCompletedToday_ReturnsTrueWhenTodayIsCompleted()
        {
            var habit = new Habit();
            habit.Records.Add(new HabitRecord { Date = DateTime.Today });

            Assert.True(habit.IsCompletedToday());
        }

        [Fact]
        public void IsCompletedToday_ReturnsFalseWhenNoTodayRecord()
        {
            var habit = new Habit();
            habit.Records.Add(new HabitRecord { Date = DateTime.Today.AddDays(-1) });

            Assert.False(habit.IsCompletedToday());
        }

        [Fact]
        public void GetCurrentStreak_ReturnsZeroWhenNoRecords()
        {
            var habit = new Habit();

            Assert.Equal(0, habit.GetCurrentStreak());
        }

        [Fact]
        public void GetCurrentStreak_ReturnsOneWhenOnlyTodayCompleted()
        {
            var habit = new Habit();
            habit.Records.Add(new HabitRecord { Date = DateTime.Today });

            Assert.Equal(1, habit.GetCurrentStreak());
        }

        [Fact]
        public void GetCurrentStreak_ReturnsTwoForTodayAndYesterday()
        {
            var habit = new Habit();
            habit.Records.Add(new HabitRecord { Date = DateTime.Today });
            habit.Records.Add(new HabitRecord { Date = DateTime.Today.AddDays(-1) });

            Assert.Equal(2, habit.GetCurrentStreak());
        }

        [Fact]
        public void GetLongestStreak_ReturnsLongestSequenceAcrossGaps()
        {
            var habit = new Habit();
            habit.Records.Add(new HabitRecord { Date = DateTime.Today.AddDays(-5) });
            habit.Records.Add(new HabitRecord { Date = DateTime.Today.AddDays(-3) });
            habit.Records.Add(new HabitRecord { Date = DateTime.Today.AddDays(-2) });
            habit.Records.Add(new HabitRecord { Date = DateTime.Today.AddDays(-1) });

            Assert.Equal(3, habit.GetLongestStreak());
        }

        [Fact]
        public void GetCompletionRate_ReturnsExpectedPercentage()
        {
            var habit = new Habit
            {
                CreatedDate = DateTime.Today.AddDays(-2)
            };

            habit.Records.Add(new HabitRecord { Date = DateTime.Today.AddDays(-1) });
            habit.Records.Add(new HabitRecord { Date = DateTime.Today });

            var rate = habit.GetCompletionRate();

            Assert.InRange(rate, 66.66, 66.67);
        }
    }
}

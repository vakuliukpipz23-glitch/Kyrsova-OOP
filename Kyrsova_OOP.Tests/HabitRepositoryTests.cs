using System;
using System.IO;
using System.Linq;
using Kyrsova_OOP.Models;
using Kyrsova_OOP.Repositories;

namespace Kyrsova_OOP.Tests
{
    public class HabitRepositoryTests
    {
        [Fact]
        public void Add_ShouldPersistHabitAndAssignId()
        {
            var (context, filePath) = DatabaseTestHelper.CreateTempDatabase();
            var repository = new HabitRepository(context);

            var habit = new Habit { Name = "Test habit" };
            repository.Add(habit);

            Assert.True(habit.Id > 0);
            Assert.Equal("Test habit", repository.GetById(habit.Id)!.Name);
        }

        [Fact]
        public void GetById_ShouldReturnHabitWithRecords()
        {
            var (context, filePath) = DatabaseTestHelper.CreateTempDatabase();
            var repository = new HabitRepository(context);

            var habit = new Habit { Name = "Record habit" };
            habit.Records.Add(new HabitRecord { Date = DateTime.Today });
            repository.Add(habit);

            var loaded = repository.GetById(habit.Id);

            Assert.NotNull(loaded);
            Assert.Equal("Record habit", loaded!.Name);
            Assert.Single(loaded.Records);
            Assert.Equal(DateTime.Today, loaded.Records.Single().Date.Date);
        }

        [Fact]
        public void Update_ShouldChangeNameAndPersist()
        {
            var (context, filePath) = DatabaseTestHelper.CreateTempDatabase();
            var repository = new HabitRepository(context);

            var habit = new Habit { Name = "Old name" };
            repository.Add(habit);
            habit.Name = "New name";
            repository.Update(habit);

            var loaded = repository.GetById(habit.Id);

            Assert.NotNull(loaded);
            Assert.Equal("New name", loaded!.Name);
        }

        [Fact]
        public void Delete_ShouldRemoveHabitAndRecords()
        {
            var (context, filePath) = DatabaseTestHelper.CreateTempDatabase();
            var repository = new HabitRepository(context);

            var habit = new Habit { Name = "Delete habit" };
            habit.Records.Add(new HabitRecord { Date = DateTime.Today });
            repository.Add(habit);

            repository.Delete(habit.Id);

            Assert.Null(repository.GetById(habit.Id));
        }

        [Fact]
        public void GetAll_ShouldReturnInsertedHabits()
        {
            var (context, filePath) = DatabaseTestHelper.CreateTempDatabase();
            var repository = new HabitRepository(context);

            var first = new Habit { Name = "First" };
            var second = new Habit { Name = "Second" };
            repository.Add(first);
            repository.Add(second);

            var all = repository.GetAll().ToList();

            Assert.Equal(2, all.Count);
            Assert.Contains(all, h => h.Name == "First");
            Assert.Contains(all, h => h.Name == "Second");
        }

        [Fact]
        public void SaveRecords_ShouldPersistNewRecordsOnUpdate()
        {
            var (context, filePath) = DatabaseTestHelper.CreateTempDatabase();
            var repository = new HabitRepository(context);

            var habit = new Habit { Name = "Record update" };
            repository.Add(habit);
            habit.Records.Add(new HabitRecord { Date = DateTime.Today });
            repository.Update(habit);

            var loaded = repository.GetById(habit.Id);

            Assert.NotNull(loaded);
            Assert.Single(loaded!.Records);
            Assert.Equal(DateTime.Today, loaded.Records.Single().Date.Date);
        }
    }
}

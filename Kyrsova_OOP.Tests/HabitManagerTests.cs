using System;
using System.IO;
using System.Linq;
using Kyrsova_OOP.Models;
using Kyrsova_OOP.Repositories;
using Kyrsova_OOP.Services;

namespace Kyrsova_OOP.Tests
{
    public class HabitManagerTests
    {
        [Fact]
        public void Create_ShouldAddHabit()
        {
            var (context, filePath) = DatabaseTestHelper.CreateTempDatabase();
            var repository = new HabitRepository(context);
            var manager = new HabitManager(repository);

            manager.Create("New habit");
            var habits = manager.GetAll().ToList();

            Assert.Single(habits);
            Assert.Equal("New habit", habits.Single().Name);
        }

        [Fact]
        public void Update_ShouldRenameHabit()
        {
            var (context, filePath) = DatabaseTestHelper.CreateTempDatabase();
            var repository = new HabitRepository(context);
            var manager = new HabitManager(repository);

            manager.Create("Old name");
            var habit = manager.GetAll().Single();
            manager.Update(habit.Id, "Updated name");

            Assert.Equal("Updated name", manager.Get(habit.Id)!.Name);
        }

        [Fact]
        public void Delete_ShouldRemoveHabit()
        {
            var (context, filePath) = DatabaseTestHelper.CreateTempDatabase();
            var repository = new HabitRepository(context);
            var manager = new HabitManager(repository);

            manager.Create("To delete");
            var habit = manager.GetAll().Single();
            manager.Delete(habit.Id);

            Assert.Null(manager.Get(habit.Id));
        }

        [Fact]
        public void Complete_ShouldAddTodayRecord()
        {
            var (context, filePath) = DatabaseTestHelper.CreateTempDatabase();
            var repository = new HabitRepository(context);
            var manager = new HabitManager(repository);

            manager.Create("Complete habit");
            var habit = manager.GetAll().Single();
            manager.Complete(habit.Id);

            var loaded = manager.Get(habit.Id);
            Assert.NotNull(loaded);
            Assert.True(loaded!.IsCompletedToday());
        }
    }
}

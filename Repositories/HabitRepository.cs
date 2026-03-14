using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Kyrsova_OOP.Models;

namespace Kyrsova_OOP.Repositories
{
    public class HabitRepository : IHabitRepository
    {
        private DatabaseContext db = new DatabaseContext();
        private bool useFile = false;
        private string filePath = string.Empty;

        public HabitRepository()
        {
            db.Initialize();
        }

        // File-backed constructor used by tests
        public HabitRepository(string filePath)
        {
            this.filePath = filePath;
            this.useFile = true;

            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, JsonSerializer.Serialize(new List<Habit>()));
            }
        }

        public List<Habit> GetAll()
    {
        if (useFile)
        {
            var text = File.ReadAllText(filePath);
            var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<List<Habit>>(text, opts) ?? new List<Habit>();
        }

        var habits = new List<Habit>();

        using var connection = db.GetConnection();
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Name, CreatedDate FROM Habits";

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            var habit = new Habit(reader.GetString(1))
            {
                Id = reader.GetInt32(0),
                CreatedDate = DateTime.Parse(reader.GetString(2))
            };

            habit.Records = GetRecords(habit.Id);

            habits.Add(habit);
        }

        return habits;
    }

        public Habit? GetById(int id)
    {
        if (useFile)
        {
            var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var list = JsonSerializer.Deserialize<List<Habit>>(File.ReadAllText(filePath), opts) ?? new List<Habit>();
            return list.Find(h => h.Id == id);
        }

        using var connection = db.GetConnection();
        connection.Open();

        var command = connection.CreateCommand();

        command.CommandText = "SELECT Id, Name, CreatedDate FROM Habits WHERE Id = $id";
        command.Parameters.AddWithValue("$id", id);

        using var reader = command.ExecuteReader();

        if (reader.Read())
        {
            var habit = new Habit(reader.GetString(1))
            {
                Id = reader.GetInt32(0),
                CreatedDate = DateTime.Parse(reader.GetString(2))
            };

            habit.Records = GetRecords(habit.Id);

            return habit;
        }

        return null;
    }

        public void Add(Habit habit)
    {
        if (useFile)
        {
            var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var list = JsonSerializer.Deserialize<List<Habit>>(File.ReadAllText(filePath), opts) ?? new List<Habit>();
            var nextId = list.Count > 0 ? list.Max(h => h.Id) + 1 : 1;
            habit.Id = nextId;
            if (habit.CreatedDate == default) habit.CreatedDate = DateTime.Today;
            list.Add(habit);
            File.WriteAllText(filePath, JsonSerializer.Serialize(list));
            return;
        }

        using var connection = db.GetConnection();
        connection.Open();

        var command = connection.CreateCommand();

        command.CommandText =
        @"INSERT INTO Habits (Name, CreatedDate)
          VALUES ($name, $date)";

        command.Parameters.AddWithValue("$name", habit.Name);
        command.Parameters.AddWithValue("$date", habit.CreatedDate.ToString());

        command.ExecuteNonQuery();
    }

        public void Update(Habit habit)
    {
        if (useFile)
        {
            var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var list = JsonSerializer.Deserialize<List<Habit>>(File.ReadAllText(filePath), opts) ?? new List<Habit>();
            var idx = list.FindIndex(h => h.Id == habit.Id);
            if (idx >= 0)
            {
                list[idx] = habit;
                File.WriteAllText(filePath, JsonSerializer.Serialize(list));
            }
            return;
        }

        using var connection = db.GetConnection();
        connection.Open();

        foreach (var record in habit.Records)
        {
            var command = connection.CreateCommand();

            command.CommandText =
            @"INSERT INTO HabitRecords (HabitId, Date)
              VALUES ($habitId, $date)";

            command.Parameters.AddWithValue("$habitId", habit.Id);
            command.Parameters.AddWithValue("$date", record.Date.ToString());

            command.ExecuteNonQuery();
        }
    }

        public void Delete(int id)
    {
        if (useFile)
        {
            var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var list = JsonSerializer.Deserialize<List<Habit>>(File.ReadAllText(filePath), opts) ?? new List<Habit>();
            list.RemoveAll(h => h.Id == id);
            File.WriteAllText(filePath, JsonSerializer.Serialize(list));
            return;
        }

        using var connection = db.GetConnection();
        connection.Open();

        var command = connection.CreateCommand();

        command.CommandText = "DELETE FROM Habits WHERE Id=$id";
        command.Parameters.AddWithValue("$id", id);

        command.ExecuteNonQuery();
    }

        private List<HabitRecord> GetRecords(int habitId)
    {
        if (useFile)
        {
            var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var list = JsonSerializer.Deserialize<List<Habit>>(File.ReadAllText(filePath), opts) ?? new List<Habit>();
            var h = list.Find(x => x.Id == habitId);
            return h?.Records ?? new List<HabitRecord>();
        }

        var records = new List<HabitRecord>();

        using var connection = db.GetConnection();
        connection.Open();

        var command = connection.CreateCommand();

        command.CommandText =
        "SELECT Date FROM HabitRecords WHERE HabitId = $id";

        command.Parameters.AddWithValue("$id", habitId);

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            records.Add(new HabitRecord
            {
                Date = DateTime.Parse(reader.GetString(0))
            });
        }

        return records;
    }
    }
}
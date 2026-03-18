using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Data.Sqlite;
using Kyrsova_OOP.Models;

namespace Kyrsova_OOP.Repositories
{
    public class HabitRepository : IHabitRepository
    {
        private readonly DatabaseContext _context;
        private static readonly object WriteSync = new();

        public HabitRepository(DatabaseContext context)
        {
            _context = context;
        }

        public IEnumerable<Habit> GetAll()
        {
            var habits = new List<Habit>();

            using var connection = _context.GetConnection();
            connection.Open();
            ConfigureConnection(connection);

            var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Name, CreatedDate FROM Habits";

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var habit = new Habit
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    CreatedDate = DateTime.Parse(reader.GetString(2))
                };

                habits.Add(habit);
            }

            foreach (var habit in habits)
            {
                habit.Records = GetRecords(connection, habit.Id);
            }

            return habits;
        }

        public Habit? GetById(int id)
        {
            using var connection = _context.GetConnection();
            connection.Open();
            ConfigureConnection(connection);

            var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Name, CreatedDate FROM Habits WHERE Id = $id";
            command.Parameters.AddWithValue("$id", id);

            using var reader = command.ExecuteReader();

            Habit? habit = null;

            if (reader.Read())
            {
                habit = new Habit
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    CreatedDate = DateTime.Parse(reader.GetString(2))
                };
            }

            if (habit is null)
            {
                return null;
            }

            habit.Records = GetRecords(connection, habit.Id);
            return habit;
        }

        public void Add(Habit habit)
        {
            lock (WriteSync)
            {
                const int maxAttempts = 2;
                for (int attempt = 1; attempt <= maxAttempts; attempt++)
                {
                    try
                    {
                        using var connection = _context.GetConnection();
                        connection.Open();
                        ConfigureConnection(connection);

                        var command = connection.CreateCommand();
                        command.CommandTimeout = 1;
                        command.CommandText = @"
                INSERT INTO Habits (Name, CreatedDate)
                VALUES ($name, $createdDate);
            ";

                        command.Parameters.AddWithValue("$name", habit.Name);
                        command.Parameters.AddWithValue("$createdDate", habit.CreatedDate.ToString("yyyy-MM-dd"));

                        command.ExecuteNonQuery();

                        command.CommandText = "SELECT last_insert_rowid();";
                        habit.Id = Convert.ToInt32((long)command.ExecuteScalar()!);

                        SaveRecords(connection, habit);
                        return;
                    }
                    catch (SqliteException ex) when (ex.SqliteErrorCode == 5 && attempt < maxAttempts)
                    {
                        Thread.Sleep(75 * attempt);
                    }
                }

                throw new InvalidOperationException("SQLite database is busy.");
            }
        }

        public void Update(Habit habit)
        {
            lock (WriteSync)
            {
                const int maxAttempts = 2;
                for (int attempt = 1; attempt <= maxAttempts; attempt++)
                {
                    try
                    {
                        using var connection = _context.GetConnection();
                        connection.Open();
                        ConfigureConnection(connection);

                        var command = connection.CreateCommand();
                        command.CommandTimeout = 1;
                        command.CommandText = @"
                UPDATE Habits 
                SET Name = $name
                WHERE Id = $id;
            ";

                        command.Parameters.AddWithValue("$name", habit.Name);
                        command.Parameters.AddWithValue("$id", habit.Id);

                        command.ExecuteNonQuery();

                        SaveRecords(connection, habit);
                        return;
                    }
                    catch (SqliteException ex) when (ex.SqliteErrorCode == 5 && attempt < maxAttempts)
                    {
                        Thread.Sleep(75 * attempt);
                    }
                }

                throw new InvalidOperationException("SQLite database is busy.");
            }
        }

        public void Delete(int id)
        {
            if (!Monitor.TryEnter(WriteSync, 300))
            {
                throw new InvalidOperationException("SQLite database is busy.");
            }

            try
            {
                const int maxAttempts = 2;

                for (int attempt = 1; attempt <= maxAttempts; attempt++)
                {
                    try
                    {
                        using var connection = _context.GetConnection();
                        connection.Open();
                        ConfigureConnection(connection);

                        using var transaction = connection.BeginTransaction();

                        var recordsCommand = connection.CreateCommand();
                        recordsCommand.Transaction = transaction;
                        recordsCommand.CommandTimeout = 1;
                        recordsCommand.CommandText = "DELETE FROM HabitRecords WHERE HabitId = $id";
                        recordsCommand.Parameters.AddWithValue("$id", id);
                        recordsCommand.ExecuteNonQuery();

                        var command = connection.CreateCommand();
                        command.Transaction = transaction;
                        command.CommandTimeout = 1;
                        command.CommandText = "DELETE FROM Habits WHERE Id = $id";
                        command.Parameters.AddWithValue("$id", id);
                        command.ExecuteNonQuery();

                        transaction.Commit();
                        return;
                    }
                    catch (SqliteException ex) when (ex.SqliteErrorCode == 5 && attempt < maxAttempts)
                    {
                        Thread.Sleep(75 * attempt);
                    }
                }

                throw new InvalidOperationException("SQLite database is locked.");
            }
            finally
            {
                Monitor.Exit(WriteSync);
            }
        }

        private static void ConfigureConnection(SqliteConnection connection)
        {
            var command = connection.CreateCommand();
            command.CommandText = "PRAGMA journal_mode = WAL; PRAGMA synchronous = NORMAL; PRAGMA busy_timeout = 250; PRAGMA foreign_keys = ON;";
            command.ExecuteNonQuery();
        }

        private static List<HabitRecord> GetRecords(SqliteConnection connection, int habitId)
        {
            var records = new List<HabitRecord>();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, Date
                FROM HabitRecords
                WHERE HabitId = $habitId
                ORDER BY Date DESC;
            ";
            command.Parameters.AddWithValue("$habitId", habitId);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                records.Add(new HabitRecord
                {
                    Id = reader.GetInt32(0),
                    Date = DateTime.Parse(reader.GetString(1))
                });
            }

            return records;
        }

        private static void SaveRecords(SqliteConnection connection, Habit habit)
        {
            var deleteCommand = connection.CreateCommand();
            deleteCommand.CommandText = "DELETE FROM HabitRecords WHERE HabitId = $habitId";
            deleteCommand.Parameters.AddWithValue("$habitId", habit.Id);
            deleteCommand.ExecuteNonQuery();

            foreach (var record in habit.Records)
            {
                var insertCommand = connection.CreateCommand();
                insertCommand.CommandText = @"
                    INSERT INTO HabitRecords (HabitId, Date)
                    VALUES ($habitId, $date);
                ";
                insertCommand.Parameters.AddWithValue("$habitId", habit.Id);
                insertCommand.Parameters.AddWithValue("$date", record.Date.ToString("yyyy-MM-dd"));
                insertCommand.ExecuteNonQuery();
            }
        }
    }
}
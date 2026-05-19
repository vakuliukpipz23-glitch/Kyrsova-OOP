using Microsoft.Data.Sqlite;

public class DatabaseContext
{
    private readonly string connectionString;

    public DatabaseContext(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public SqliteConnection GetConnection()
    {
        return new SqliteConnection(connectionString);
    }

    public void Initialize()
    {
        using var connection = GetConnection();
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
CREATE TABLE IF NOT EXISTS Habits (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    CreatedDate TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS HabitRecords (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    HabitId INTEGER NOT NULL,
    Date TEXT NOT NULL,
    FOREIGN KEY (HabitId) REFERENCES Habits(Id) ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS IX_HabitRecords_HabitId
    ON HabitRecords(HabitId);
";
        command.ExecuteNonQuery();
    }
}

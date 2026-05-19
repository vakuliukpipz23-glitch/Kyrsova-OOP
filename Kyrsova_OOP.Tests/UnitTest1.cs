using System;
using System.IO;

namespace Kyrsova_OOP.Tests;

public static class DatabaseTestHelper
{
    public static (DatabaseContext Context, string FilePath) CreateTempDatabase()
    {
        var filePath = Path.Combine(Path.GetTempPath(), $"Kyrsova_OOP_Tests_{Guid.NewGuid():N}.db");
        var context = new DatabaseContext($"Data Source={filePath}");
        context.Initialize();
        return (context, filePath);
    }
}

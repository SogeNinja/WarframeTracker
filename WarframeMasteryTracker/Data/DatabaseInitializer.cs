using Dapper;
using Microsoft.Data.Sqlite;
using System.IO;

public static class DatabaseInitializer
{
    private static readonly string DbFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "warframe_mastery.db");

    public static void Initialize()
    {
        bool dbExists = File.Exists(DbFile);

        using var connection = new SqliteConnection($"Data Source={DbFile}");
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS Items (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Category TEXT NOT NULL,
                Type TEXT,
                MasteryReq INTEGER
            );

            CREATE TABLE IF NOT EXISTS UserProgress (
                ItemId INTEGER PRIMARY KEY,
                IsMastered INTEGER NOT NULL
            );

            CREATE TABLE IF NOT EXISTS UserProfile (
                Id INTEGER PRIMARY KEY,
                CurrentMastery INTEGER NOT NULL
            );

            INSERT OR IGNORE INTO UserProfile (Id, CurrentMastery)
            VALUES (1, 0);
        ";
        command.ExecuteNonQuery();

        // 🔹 IMPORTAR SOLO SI LA TABLA ESTÁ VACÍA
        var itemCount = connection.QuerySingle<int>("SELECT COUNT(1) FROM Items");
        if (itemCount == 0)
        {
            WarframeItemsImporter.Import();
        }

    }
}

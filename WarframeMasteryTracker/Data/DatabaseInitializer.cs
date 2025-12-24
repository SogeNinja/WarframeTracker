using Microsoft.Data.Sqlite;

public static class DatabaseInitializer
{
    private const string DbFile = "warframe_mastery.db";

    public static void Initialize()
    {
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
    }
}

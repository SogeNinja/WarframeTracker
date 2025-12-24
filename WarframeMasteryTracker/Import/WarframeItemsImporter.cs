using System.IO;
using System.Text.Json;
using Dapper;
using Microsoft.Data.Sqlite;
using System.Windows;


public static class WarframeItemsImporter
{
    private const string DbFile = "warframe_mastery.db";
    private const string JsonPath = "Assets/json";

    public static void Import()
    {
        MessageBox.Show("IMPORTER SE ESTÁ EJECUTANDO");

        using var connection = new SqliteConnection($"Data Source={DbFile}");
        connection.Open();

        ImportFile(connection, "Primary.json", "Primary");
        ImportFile(connection, "Secondary.json", "Secondary");
        ImportFile(connection, "Melee.json", "Melee");
        ImportFile(connection, "Warframes.json", "Warframe");
    }

    private static void ImportFile(SqliteConnection conn, string fileName, string category)
    {
        var fullPath = Path.Combine(JsonPath, fileName);
        var json = File.ReadAllText(fullPath);

        using var doc = JsonDocument.Parse(json);

        foreach (var element in doc.RootElement.EnumerateArray())
        {
            if (!element.TryGetProperty("name", out var nameProp))
                continue;

            string name = nameProp.GetString() ?? "";
            string type = element.TryGetProperty("type", out var typeProp)
                ? typeProp.GetString() ?? ""
                : "";

            int? masteryReq = element.TryGetProperty("masteryReq", out var mrProp)
                ? mrProp.GetInt32()
                : null;

            conn.Execute(@"
                INSERT INTO Items (Name, Category, Type, MasteryReq)
                VALUES (@Name, @Category, @Type, @MasteryReq)
            ", new
            {
                Name = name,
                Category = category,
                Type = type,
                MasteryReq = masteryReq
            });
        }
    }
}

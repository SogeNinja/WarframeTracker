using Dapper;
using Microsoft.Data.Sqlite;
using WarframeMasteryTracker.Models;

namespace WarframeMasteryTracker.Data;

public class ItemsRepository
{
    private readonly string _connectionString =
        "Data Source=warframe_mastery.db";

    private SqliteConnection GetConnection()
        => new SqliteConnection(_connectionString);

    /// <summary>
    /// Query principal:
    /// - Ordenado por MR requerida
    /// - Luego por nombre
    /// - Filtro por búsqueda y type
    /// </summary>
    public IEnumerable<Item> GetItems(
        string? searchText,
        string? typeFilter,
        string? categoryFilter
    )
    {
        using var connection = GetConnection();

        var sql = @"
        SELECT
            i.Id,
            i.Name,
            i.Category,
            i.Type,
            i.MasteryReq,
            COALESCE(up.IsMastered, 0) AS IsMastered
        FROM Items i
        LEFT JOIN UserProgress up ON up.ItemId = i.Id
        WHERE 1 = 1

        ";

        var parameters = new DynamicParameters();

        // 🔍 Buscador por nombre
        if (!string.IsNullOrWhiteSpace(searchText))
        {
            sql += " AND Name LIKE @Search ";
            parameters.Add("Search", $"%{searchText}%");
        }

        // 🧩 Filtro por type
        if (!string.IsNullOrWhiteSpace(typeFilter) && typeFilter != "All")
        {
            sql += " AND Type = @Type ";
            parameters.Add("Type", typeFilter);
        }

        // 🧩 Filtro por category
        if (!string.IsNullOrWhiteSpace(categoryFilter) && categoryFilter != "All")
        {
            sql += " AND Category = @Category ";
            parameters.Add("Category", categoryFilter);
        }

        // 🔢 Orden FINAL
        sql += @"
        ORDER BY 
            COALESCE(MasteryReq, 0) ASC,
            Name ASC
        ";

        return connection.Query<Item>(sql, parameters);
    }

    public IEnumerable<string> GetAvailableTypes(
        string? categoryFilter
    )
    {
        using var connection = GetConnection();

        var parameters = new DynamicParameters();

        var sql = @"
            SELECT DISTINCT Type
            FROM Items
            WHERE 1 = 1
              AND Type IS NOT NULL
              AND Type != ''
            ";

        if (!string.IsNullOrWhiteSpace(categoryFilter))
        {
            sql += " AND Category = @Category COLLATE NOCASE";
            parameters.Add("Category", categoryFilter);
        }
        sql += " ORDER BY Type ASC ";

        return connection.Query<string>(sql, parameters);
    }

    public IEnumerable<string> GetAvailableCategories()
    {
        using var connection = GetConnection();

        var sql = @"
            SELECT DISTINCT Category
            FROM Items
            WHERE Category IS NOT NULL
              AND Category != ''
            ORDER BY Category ASC
            ";

        return connection.Query<string>(sql);
    }

    public void SetMastered(
        int itemId,
        bool isMastered
    )
    {
        using var connection = GetConnection();

        var sql = @"
            INSERT INTO UserProgress (ItemId, IsMastered)
            VALUES (@ItemId, @IsMastered)
            ON CONFLICT(ItemId)
            DO UPDATE SET IsMastered = @IsMastered;
        ";

        connection.Execute(sql, new
        {
            ItemId = itemId,
            IsMastered = isMastered ? 1 : 0
        });
    }

}

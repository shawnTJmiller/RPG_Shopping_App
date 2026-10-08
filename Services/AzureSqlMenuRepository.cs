using GurpsShoppingApp.Models;
using Microsoft.Data.SqlClient;

namespace GurpsShoppingApp.Services;

public sealed class AzureSqlMenuRepository
{
    private readonly AzureSqlConnectionSettings _settings;

    public AzureSqlMenuRepository(AzureSqlConnectionSettings? settings = null)
    {
        _settings = settings ?? AzureSqlConnectionSettings.FromEnvironment();
    }

    public async Task<IReadOnlyList<DatabaseMenuItem>> LoadMenuAsync(string menuName, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = await _settings.CreateOpenConnectionAsync(cancellationToken);
            await using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, MenuName, Label, TargetScreen, IsExit, SortOrder
                FROM dbo.MenuItems
                WHERE MenuName = @MenuName
                ORDER BY SortOrder, Id";
            command.Parameters.AddWithValue("@MenuName", menuName);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            var items = new List<DatabaseMenuItem>();

            while (await reader.ReadAsync(cancellationToken))
            {
                items.Add(new DatabaseMenuItem
                {
                    Id = reader.GetInt32(0),
                    MenuName = reader.GetString(1),
                    Label = reader.GetString(2),
                    TargetScreen = reader.IsDBNull(3) ? null : reader.GetString(3),
                    IsExit = reader.GetBoolean(4),
                    SortOrder = reader.GetInt32(5)
                });
            }

            return items;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Database menu load failed: {ex.Message}");
            return Array.Empty<DatabaseMenuItem>();
        }
    }
}

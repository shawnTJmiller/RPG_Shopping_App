using GurpsShoppingApp.Services;
using Xunit;

namespace GurpsShoppingApp.Tests;

public sealed class AzureSqlConnectionSettingsTests
{
    [Fact]
    public void BuildConnectionString_UsesEncryptedSettings()
    {
        var settings = new AzureSqlConnectionSettings
        {
            Server = "example.database.windows.net",
            Database = "shopping",
            UserId = "appuser",
            Password = "secret"
        };

        var connectionString = settings.BuildConnectionString();

        Assert.Contains("Encrypt=True", connectionString, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("TrustServerCertificate=False", connectionString, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Initial Catalog=shopping", connectionString, StringComparison.OrdinalIgnoreCase);
    }
}

using GurpsShoppingApp.Services;
using Microsoft.Data.SqlClient;
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

        var builder = new SqlConnectionStringBuilder(settings.BuildConnectionString());

        Assert.True(builder.Encrypt);
        Assert.False(builder.TrustServerCertificate);
        Assert.Equal("shopping", builder.InitialCatalog);
    }

    [Fact]
    public void BuildConnectionString_HardensProvidedConnectionString()
    {
        var settings = new AzureSqlConnectionSettings
        {
            ConnectionString = "Server=tcp:example.database.windows.net;Database=shopping;TrustServerCertificate=True",
            UseManagedIdentity = true
        };

        var builder = new SqlConnectionStringBuilder(settings.BuildConnectionString());

        Assert.True(builder.Encrypt);
        Assert.False(builder.TrustServerCertificate);
        Assert.Equal(SqlAuthenticationMethod.ActiveDirectoryDefault, builder.Authentication);
        Assert.Equal(5, builder.ConnectRetryCount);
    }
}

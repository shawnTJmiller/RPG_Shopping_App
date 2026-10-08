using Azure.Core;
using Azure.Identity;
using Microsoft.Data.SqlClient;

namespace GurpsShoppingApp.Services;

public sealed class AzureSqlConnectionSettings
{
    public string? ConnectionString { get; init; }

    public string? Server { get; init; }

    public string? Database { get; init; }

    public string? UserId { get; init; }

    public string? Password { get; init; }

    public bool UseManagedIdentity { get; init; }

    public static AzureSqlConnectionSettings FromEnvironment()
    {
        return new AzureSqlConnectionSettings
        {
            ConnectionString = Environment.GetEnvironmentVariable("AZURE_SQL_CONNECTIONSTRING"),
            Server = Environment.GetEnvironmentVariable("AZURE_SQL_SERVER"),
            Database = Environment.GetEnvironmentVariable("AZURE_SQL_DATABASE"),
            UserId = Environment.GetEnvironmentVariable("AZURE_SQL_USER"),
            Password = Environment.GetEnvironmentVariable("AZURE_SQL_PASSWORD"),
            UseManagedIdentity = string.Equals(Environment.GetEnvironmentVariable("AZURE_SQL_USE_MANAGED_IDENTITY"), "true", StringComparison.OrdinalIgnoreCase)
        };
    }

    public string BuildConnectionString()
    {
        if (!string.IsNullOrWhiteSpace(ConnectionString))
        {
            return ConnectionString;
        }

        if (string.IsNullOrWhiteSpace(Server) || string.IsNullOrWhiteSpace(Database))
        {
            throw new InvalidOperationException("Set either AZURE_SQL_CONNECTIONSTRING or both AZURE_SQL_SERVER and AZURE_SQL_DATABASE.");
        }

        var builder = new SqlConnectionStringBuilder
        {
            DataSource = Server,
            InitialCatalog = Database,
            Encrypt = true,
            ConnectTimeout = 30
        };

        if (UseManagedIdentity)
        {
            builder.Authentication = SqlAuthenticationMethod.ActiveDirectoryDefault;
        }
        else if (!string.IsNullOrWhiteSpace(UserId) && !string.IsNullOrWhiteSpace(Password))
        {
            builder.UserID = UserId;
            builder.Password = Password;
            builder.Authentication = SqlAuthenticationMethod.SqlPassword;
        }

        var connectionString = builder.ConnectionString;
        return connectionString.Contains("TrustServerCertificate=", StringComparison.OrdinalIgnoreCase)
            ? connectionString
            : $"{connectionString};TrustServerCertificate=False";
    }

    public async Task<SqlConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connection = new SqlConnection(BuildConnectionString());

        if (UseManagedIdentity)
        {
            var credential = new DefaultAzureCredential();
            var token = await credential.GetTokenAsync(new TokenRequestContext(new[] { "https://database.windows.net/.default" }), cancellationToken);
            connection.AccessToken = token.Token;
        }

        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}

using Microsoft.Data.SqlClient;

namespace GurpsShoppingApp.Services;

public sealed class AzureSqlConnectionSettings
{
    private const string DefaultServer = "rec-ttg-stjm.database.windows.net";
    private const string DefaultDatabase = "gurps-shopping-app";

    public string? ConnectionString { get; init; }

    public string? Server { get; init; }

    public string? Database { get; init; }

    public string? UserId { get; init; }

    public string? Password { get; init; }

    public bool UseManagedIdentity { get; init; }

    public static AzureSqlConnectionSettings FromEnvironment()
    {
        var managedIdentitySetting = Environment.GetEnvironmentVariable("AZURE_SQL_USE_MANAGED_IDENTITY");
        var userId = Environment.GetEnvironmentVariable("AZURE_SQL_USER");
        var password = Environment.GetEnvironmentVariable("AZURE_SQL_PASSWORD");
        return new AzureSqlConnectionSettings
        {
            ConnectionString = Environment.GetEnvironmentVariable("AZURE_SQL_CONNECTIONSTRING"),
            Server = Environment.GetEnvironmentVariable("AZURE_SQL_SERVER") ?? DefaultServer,
            Database = Environment.GetEnvironmentVariable("AZURE_SQL_DATABASE") ?? DefaultDatabase,
            UserId = userId,
            Password = password,
            UseManagedIdentity = managedIdentitySetting is null
                ? string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(password)
                : string.Equals(managedIdentitySetting, "true", StringComparison.OrdinalIgnoreCase)
        };
    }

    public string BuildConnectionString()
    {
        if (!string.IsNullOrWhiteSpace(ConnectionString))
        {
            var configuredBuilder = new SqlConnectionStringBuilder(ConnectionString)
            {
                Encrypt = true,
                TrustServerCertificate = false,
                ConnectTimeout = 30,
                ConnectRetryCount = 5,
                ConnectRetryInterval = 10
            };

            if (UseManagedIdentity && string.IsNullOrWhiteSpace(configuredBuilder.UserID))
            {
                configuredBuilder.Authentication = SqlAuthenticationMethod.ActiveDirectoryDefault;
            }

            return configuredBuilder.ConnectionString;
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
            TrustServerCertificate = false,
            ConnectTimeout = 30,
            ConnectRetryCount = 5,
            ConnectRetryInterval = 10,
            ApplicationName = "GurpsShoppingApp"
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

        return builder.ConnectionString;
    }

    public async Task<SqlConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connection = new SqlConnection(BuildConnectionString());
        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}

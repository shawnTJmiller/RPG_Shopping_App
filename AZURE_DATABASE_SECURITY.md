# Azure Database connection security

The app defaults to the SQL logical server `rec-ttg-stjm.database.windows.net` and database `gurps-shopping-app`. Override these values if `rec-ttg-stjm` is a resource group rather than the SQL server name.

## Preferred authentication: Microsoft Entra ID

For local development, sign in with Azure CLI and let `DefaultAzureCredential` use that identity. On Azure hosting, enable a managed identity for the app instead. Configure the SQL logical server with a Microsoft Entra administrator, then create a database user for the developer or managed identity and grant it only the permissions it needs (for example, `SELECT` on `dbo.MenuItems`). The app uses encrypted connections, validates the server certificate, and retries transient SQL failures.

Optional Windows PowerShell overrides:

```powershell
$env:AZURE_SQL_SERVER = "rec-ttg-stjm.database.windows.net"
$env:AZURE_SQL_DATABASE = "gurps-shopping-app"
$env:AZURE_SQL_USE_MANAGED_IDENTITY = "true"
```

The defaults use Azure identity when no SQL credentials are supplied. If you must use SQL authentication, supply the credentials through a secret store or environment variables and disable identity authentication:

```powershell
$env:AZURE_SQL_USER = "appuser"
$env:AZURE_SQL_PASSWORD = "<secret-from-key-vault>"
$env:AZURE_SQL_USE_MANAGED_IDENTITY = "false"
```

Never commit credentials or a populated connection string to source control. Ensure the Azure SQL firewall permits the app's outbound network (or use a private endpoint), and enable auditing and threat detection where appropriate.

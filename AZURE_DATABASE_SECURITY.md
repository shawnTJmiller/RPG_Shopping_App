# Azure Database connection security

1. Prefer Microsoft Entra ID / managed identity over SQL usernames and passwords.
2. Store secrets in Azure Key Vault and load them at runtime from environment variables.
3. Use a connection string that enables encryption and disables certificate trust bypass:
   - `Encrypt=True`
   - `TrustServerCertificate=False`
4. Restrict Azure SQL firewall rules to the app's outbound IPs or private endpoint.
5. Grant the app the minimum required database permissions. A read-only app account is usually enough for menu data.
6. Rotate secrets regularly and enable Azure SQL auditing and threat detection.

Example environment variables for Windows PowerShell:

```powershell
$env:AZURE_SQL_SERVER = "yourserver.database.windows.net"
$env:AZURE_SQL_DATABASE = "yourdatabase"
$env:AZURE_SQL_USE_MANAGED_IDENTITY = "true"
```

If you must use SQL authentication, set these instead:

```powershell
$env:AZURE_SQL_USER = "appuser"
$env:AZURE_SQL_PASSWORD = "<secret-from-key-vault>"
```

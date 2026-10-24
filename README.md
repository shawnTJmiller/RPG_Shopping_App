# GURPS Shopping App

A .NET 8 console application that provides a keyboard-navigable shopping menu for GURPS. The home menu can load its entries from Azure SQL, allowing menu labels, ordering, exit behavior, and destination screen titles to be maintained in the database. If the database is unavailable or no home menu entries are returned, the app displays a built-in fallback menu.

## Features

- Text-based menu navigation using arrow keys, Enter, or a menu item's number.
- Azure SQL-backed `HOME` menu items, ordered by sort order and ID.
- Built-in `BUY`, `SELL`, `TRADE`, `ADD ITEMS TO LIST`, and `CLOSE APP` fallback entries.
- Azure SQL connectivity with encrypted connections, certificate validation, connection retries, and retry of transient menu-query failures.
- Microsoft Entra ID authentication by default when SQL credentials are not provided; SQL authentication is supported for environments that require it.

## Requirements

- .NET 8 SDK.
- For database-backed menus: network access to the configured Azure SQL server and a database identity authorized to read `dbo.MenuItems`.
- For local Microsoft Entra authentication: Azure CLI installed and signed in with an authorized account (`az login`).

The default SQL endpoint is `rec-ttg-stjm.database.windows.net`, database `gurps-shopping-app`. These defaults can be overridden using the environment variables below. The Azure SQL server firewall must allow the machine's outbound network, or the app must connect through an approved private endpoint.

## Launch

From the repository directory, restore packages and run the app:

```powershell
dotnet restore
dotnet run --project .\GurpsShoppingApp.csproj
```

With the defaults, the app attempts to connect using the local Azure identity. If Azure SQL is not reachable or the query fails, the home menu falls back to the built-in options.

## Database configuration

To override the server or database in PowerShell:

```powershell
$env:AZURE_SQL_SERVER = "your-server.database.windows.net"
$env:AZURE_SQL_DATABASE = "your-database"
$env:AZURE_SQL_USE_MANAGED_IDENTITY = "true"
dotnet run --project .\GurpsShoppingApp.csproj
```

For local Microsoft Entra authentication, sign in with Azure CLI first. On an Azure host, enable a managed identity for the application. In either case, configure a Microsoft Entra administrator on the SQL logical server, create a database user for the identity, and grant only the required permissions. The menu currently needs read access to `dbo.MenuItems`.

SQL authentication is available if needed. Use a secure secret store or set credentials only in the current process environment; do not commit credentials or a populated connection string:

```powershell
$env:AZURE_SQL_USER = "appuser"
$env:AZURE_SQL_PASSWORD = "<secret>"
$env:AZURE_SQL_USE_MANAGED_IDENTITY = "false"
dotnet run --project .\GurpsShoppingApp.csproj
```

Supported environment variables:

| Variable | Purpose |
| --- | --- |
| `AZURE_SQL_CONNECTIONSTRING` | Optional full connection string; TLS and retry settings are enforced by the app. |
| `AZURE_SQL_SERVER` | SQL logical server hostname. |
| `AZURE_SQL_DATABASE` | Database name. |
| `AZURE_SQL_USE_MANAGED_IDENTITY` | Set to `true` to use Azure identity or `false` to disable it. If omitted, identity is used unless both SQL username and password are supplied. |
| `AZURE_SQL_USER` | SQL authentication username. |
| `AZURE_SQL_PASSWORD` | SQL authentication password. |

More security and provisioning guidance is in [AZURE_DATABASE_SECURITY.md](AZURE_DATABASE_SECURITY.md).

## Menu data expectations

The app reads the `dbo.MenuItems` table using these columns:

- `Id` (integer)
- `MenuName` (string; the home screen loads rows where this is `HOME`)
- `Label` (string)
- `TargetScreen` (nullable string)
- `IsExit` (boolean)
- `SortOrder` (integer)

Rows are ordered by `SortOrder` and then `Id`. Selecting a row marked `IsExit` closes the app; otherwise its `TargetScreen` (or its label when the target is empty) becomes the displayed submenu title.

## Run tests

```powershell
dotnet test .\GurpsShoppingApp.Tests\GurpsShoppingApp.Tests.csproj
```
